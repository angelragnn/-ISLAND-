using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneData
{
    public int collectiblesCollected = 0;
    public int totalCollectibles = 0;
    public int livesRemaining = 3;
    public int livesLost = 0;
    public int deathCount = 0;
    public bool completed = false;
    public float completionTime = 0f;
    public int score = 0;
}

[Serializable]
public class SessionStats
{
    public float totalPlayTime = 0f;
    public int totalDeaths = 0;
    public int gamesCompleted = 0;
}

[Serializable]
public class GameData
{
    public string playerName = "Jugador";
    public string lastScenePlayed = "";
    public string timestamp = "";
    public SceneData scene1 = new SceneData();
    public SceneData scene2 = new SceneData();
    public SceneData scene3 = new SceneData();
    public SceneData scene4 = new SceneData();
    public SessionStats sessionStats = new SessionStats();
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion de Vidas")]
    public int maxLives = 3;

    [HideInInspector] public int currentLives;
    [HideInInspector] public int currentScore;
    [HideInInspector] public int currentDeaths;
    [HideInInspector] public int collectiblesCollected;
    [HideInInspector] public int totalCollectiblesInScene;
    [HideInInspector] public float sceneTimer;
    [HideInInspector] public bool isTimerRunning;

    private GameData gameData;
    private string savePath;
    private int currentSceneIndex = 0;

    public event Action<int> OnLivesChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnDeathsChanged;
    public event Action<int, int> OnCollectiblePickedUp;
    public event Action OnPlayerDied;
    public event Action OnSceneCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        savePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        LoadGame();
    }

    void Start() { InitScene(); }

    void Update()
    {
        if (isTimerRunning) sceneTimer += Time.deltaTime;
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) { InitScene(); }

    void InitScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        gameData.lastScenePlayed = sceneName;
        gameData.timestamp = DateTime.Now.ToString("o");
        currentSceneIndex = GetSceneIndex(sceneName);

        if (currentSceneIndex > 0)
        {
            SceneData sd = GetCurrentSceneData();
            currentLives = sd.livesRemaining > 0 ? sd.livesRemaining : maxLives;
            currentScore = sd.score;
            currentDeaths = sd.deathCount;
            collectiblesCollected = sd.collectiblesCollected;
            totalCollectiblesInScene = GameObject.FindGameObjectsWithTag("Collectable").Length;
            sd.totalCollectibles = totalCollectiblesInScene;
            sceneTimer = 0f;
            isTimerRunning = true;
            SaveGame();
        }
    }

    int GetSceneIndex(string sceneName)
    {
        switch (sceneName)
        {
            case "2BOSQUE": return 1;
            case "3Dugeon": return 2;
            case "4DugeonHielo": return 3;
            case "5DugeonFuego": return 4;
            default: return 0;
        }
    }

    public SceneData GetCurrentSceneData()
    {
        switch (currentSceneIndex)
        {
            case 1: return gameData.scene1;
            case 2: return gameData.scene2;
            case 3: return gameData.scene3;
            case 4: return gameData.scene4;
            default: return null;
        }
    }

    public void NotifyPlayerDied() { OnPlayerDied?.Invoke(); }
    public void RegisterCollection() { OnCollectibleCollected(10); }

    public void OnCollectibleCollected(int points)
    {
        collectiblesCollected++;
        currentScore += points;
        SceneData sd = GetCurrentSceneData();
        if (sd != null) { sd.collectiblesCollected = collectiblesCollected; sd.score = currentScore; }
        OnCollectiblePickedUp?.Invoke(collectiblesCollected, totalCollectiblesInScene);
        OnScoreChanged?.Invoke(currentScore);
        if (collectiblesCollected >= totalCollectiblesInScene) CompleteScene();
        SaveGame();
    }

    public void PlayerDied()
    {
        currentDeaths++;
        currentLives--;
        SceneData sd = GetCurrentSceneData();
        if (sd != null) { sd.deathCount = currentDeaths; sd.livesLost++; sd.livesRemaining = currentLives; }
        gameData.sessionStats.totalDeaths++;
        OnDeathsChanged?.Invoke(currentDeaths);
        OnLivesChanged?.Invoke(currentLives);
        OnPlayerDied?.Invoke();
        SaveGame();
        if (currentLives <= 0) GameOver();
        else StartCoroutine(RespawnDelay());
    }

    public void AddScore(int points)
    {
        currentScore += points;
        SceneData sd = GetCurrentSceneData();
        if (sd != null) sd.score = currentScore;
        OnScoreChanged?.Invoke(currentScore);
        SaveGame();
    }

    public void CompleteScene()
    {
        isTimerRunning = false;
        SceneData sd = GetCurrentSceneData();
        if (sd != null) { sd.completed = true; sd.completionTime = sceneTimer; sd.livesRemaining = currentLives; }
        gameData.sessionStats.totalPlayTime += sceneTimer;
        gameData.sessionStats.gamesCompleted++;
        OnSceneCompleted?.Invoke();
        SaveGame();
    }

    void GameOver()
    {
        isTimerRunning = false;
        StartCoroutine(ReloadSceneDelay(2f));
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CheckpointManager cp = FindObjectOfType<CheckpointManager>();
        if (cp != null) cp.RespawnPlayer();
    }

    IEnumerator ReloadSceneDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneData sd = GetCurrentSceneData();
        if (sd != null) { sd.livesRemaining = maxLives; sd.collectiblesCollected = 0; sd.score = 0; }
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGame()
    {
        gameData.timestamp = DateTime.Now.ToString("o");
        File.WriteAllText(savePath, JsonUtility.ToJson(gameData, true));
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
            gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(savePath));
        else
            gameData = new GameData();
    }

    public void ResetAllData() { gameData = new GameData(); SaveGame(); }

    public SceneData GetSceneData(int index)
    {
        switch (index)
        {
            case 1: return gameData.scene1;
            case 2: return gameData.scene2;
            case 3: return gameData.scene3;
            case 4: return gameData.scene4;
            default: return null;
        }
    }

    public GameData GetGameData() => gameData;
}