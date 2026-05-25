using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
//  ESTRUCTURAS DE DATOS JSON
// ============================================================
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

    // Una entrada por escena jugable (indices 0-4 = escenas 1-5)
    public SceneData scene1 = new SceneData(); // 2BOSQUE
    public SceneData scene2 = new SceneData(); // 3Dugeon
    public SceneData scene3 = new SceneData(); // 4DugeonHielo
    public SceneData scene4 = new SceneData(); // 5DugeonFuego

    public SessionStats sessionStats = new SessionStats();
}

// ============================================================
//  GAME MANAGER
// ============================================================
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Configuracion ---
    [Header("Configuracion de Vidas")]
    public int maxLives = 3;

    [Header("Raycast Recoleccion")]
    public float raycastDistance = 4f;          // 3-5 unidades segun enunciado
    public LayerMask collectibleLayer;
    public Camera playerCamera;

    // --- Estado en tiempo real ---
    [HideInInspector] public int currentLives;
    [HideInInspector] public int currentScore;
    [HideInInspector] public int currentDeaths;
    [HideInInspector] public int collectiblesCollected;
    [HideInInspector] public int totalCollectiblesInScene;
    [HideInInspector] public float sceneTimer;
    [HideInInspector] public bool isTimerRunning;

    // --- JSON ---
    private GameData gameData;
    private string savePath;

    // --- Escena actual (indice 1-4 para escenas jugables) ---
    private int currentSceneIndex = 0;

    // ============================================================
    //  EVENTOS (para UI y otros sistemas)
    // ============================================================
    public event Action<int> OnLivesChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnDeathsChanged;
    public event Action<int, int> OnCollectiblePickedUp;   // recogido, total
    public event Action OnPlayerDied;
    public event Action OnSceneCompleted;

    // ============================================================
    //  UNITY LIFECYCLE
    // ============================================================
    void Awake()
    {
        // Singleton persistente entre escenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        LoadGame();
    }

    void Start()
    {
        InitScene();
    }

    void Update()
    {
        // Timer
        if (isTimerRunning)
            sceneTimer += Time.deltaTime;

        // Raycast de recoleccion (boton E o clic izquierdo)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            TryCollectWithRaycast();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitScene();
    }

    // ============================================================
    //  INICIALIZACION DE ESCENA
    // ============================================================
    void InitScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        gameData.lastScenePlayed = sceneName;
        gameData.timestamp = DateTime.Now.ToString("o");

        // Mapear nombre de escena a indice
        currentSceneIndex = GetSceneIndex(sceneName);

        if (currentSceneIndex > 0)
        {
            SceneData sd = GetCurrentSceneData();
            currentLives = sd.livesRemaining > 0 ? sd.livesRemaining : maxLives;
            currentScore = sd.score;
            currentDeaths = sd.deathCount;
            collectiblesCollected = sd.collectiblesCollected;

            // Contar recolectables en escena automaticamente
            totalCollectiblesInScene = GameObject.FindGameObjectsWithTag("Recolectable").Length;
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
            default: return 0; // Menu u otras
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

    // ============================================================
    //  RAYCAST RECOLECCION (3-5 unidades)
    // ============================================================
    void TryCollectWithRaycast()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, collectibleLayer))
        {
            if (hit.collider.CompareTag("Recolectable"))
            {
                Collectible col = hit.collider.GetComponent<Collectible>();
                if (col != null)
                    col.Collect();
            }
        }
    }

    // ============================================================
    //  METODOS PUBLICOS — llamados desde otros scripts
    // ============================================================

    /// <summary>Llamado desde Collectible.cs al recoger un objeto</summary>
    public void OnCollectibleCollected(int points)
    {
        collectiblesCollected++;
        currentScore += points;

        SceneData sd = GetCurrentSceneData();
        if (sd != null)
        {
            sd.collectiblesCollected = collectiblesCollected;
            sd.score = currentScore;
        }

        OnCollectiblePickedUp?.Invoke(collectiblesCollected, totalCollectiblesInScene);
        OnScoreChanged?.Invoke(currentScore);

        // Verificar si se completaron todos
        if (collectiblesCollected >= totalCollectiblesInScene)
            CompleteScene();

        SaveGame();
    }

    /// <summary>Llamar cuando el jugador muere / cae al vacio</summary>
    public void PlayerDied()
    {
        currentDeaths++;
        currentLives--;

        SceneData sd = GetCurrentSceneData();
        if (sd != null)
        {
            sd.deathCount = currentDeaths;
            sd.livesLost++;
            sd.livesRemaining = currentLives;
        }

        gameData.sessionStats.totalDeaths++;

        OnDeathsChanged?.Invoke(currentDeaths);
        OnLivesChanged?.Invoke(currentLives);
        OnPlayerDied?.Invoke();

        SaveGame();

        if (currentLives <= 0)
            GameOver();
        else
            StartCoroutine(RespawnDelay());
    }

    /// <summary>Sumar puntos directamente (trampas, eventos, etc.)</summary>
    public void AddScore(int points)
    {
        currentScore += points;
        SceneData sd = GetCurrentSceneData();
        if (sd != null) sd.score = currentScore;
        OnScoreChanged?.Invoke(currentScore);
        SaveGame();
    }

    /// <summary>Marcar escena como completada</summary>
    public void CompleteScene()
    {
        isTimerRunning = false;
        SceneData sd = GetCurrentSceneData();
        if (sd != null)
        {
            sd.completed = true;
            sd.completionTime = sceneTimer;
            sd.livesRemaining = currentLives;
        }

        gameData.sessionStats.totalPlayTime += sceneTimer;
        gameData.sessionStats.gamesCompleted++;

        OnSceneCompleted?.Invoke();
        SaveGame();
    }

    // ============================================================
    //  GAME OVER Y RESPAWN
    // ============================================================
    void GameOver()
    {
        isTimerRunning = false;
        Debug.Log("GAME OVER");
        // Aqui puedes cargar una escena de GameOver o reiniciar
        StartCoroutine(ReloadSceneDelay(2f));
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(1.5f);
        // Reinicia posicion del jugador al ultimo checkpoint
        CheckpointManager cp = FindObjectOfType<CheckpointManager>();
        if (cp != null) cp.RespawnPlayer();
    }

    IEnumerator ReloadSceneDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Resetear vidas para la escena
        SceneData sd = GetCurrentSceneData();
        if (sd != null)
        {
            sd.livesRemaining = maxLives;
            sd.collectiblesCollected = 0;
            sd.score = 0;
        }
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ============================================================
    //  JSON — GUARDAR Y CARGAR
    // ============================================================
    public void SaveGame()
    {
        gameData.timestamp = DateTime.Now.ToString("o");
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[GameManager] Guardado en: {savePath}");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("[GameManager] Datos cargados desde JSON.");
        }
        else
        {
            gameData = new GameData();
            Debug.Log("[GameManager] No hay save previo, creando nuevo GameData.");
        }
    }

    public void ResetAllData()
    {
        gameData = new GameData();
        SaveGame();
    }

    // Acceso publico a datos de escena (para UI)
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