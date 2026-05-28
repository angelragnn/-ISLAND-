using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoGameOver;
    [SerializeField][Range(0f, 1f)] private float volumen = 0.4f;
    [SerializeField] private int vidasMaximas = 3;

    private int vidas;
    public int Vidas => vidas;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            vidas = (sd != null && sd.livesRemaining > 0) ? sd.livesRemaining : vidasMaximas;
        }
        else
        {
            vidas = vidasMaximas;
        }

        UIManager.Instance?.ActualizarVidas(vidas);
    }

    public void PerderVida()
    {
        if (vidas <= 0) return;
        vidas--;

        if (GameManager.Instance != null)
        {
            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            if (sd != null)
            {
                sd.livesRemaining = vidas;
                sd.livesLost++;
                sd.deathCount++;
            }
            GameManager.Instance.currentLives = vidas;
            GameManager.Instance.currentDeaths++;
            GameManager.Instance.NotifyPlayerDied();
            GameManager.Instance.SaveGame();
        }

        UIManager.Instance?.ActualizarVidas(vidas);

        if (vidas <= 0)
        {
            AudioSource.PlayClipAtPoint(sonidoGameOver, Camera.main.transform.position, volumen);
            UIManager.Instance?.MostrarGameOver();
        }
        else
        {
            AudioSource.PlayClipAtPoint(sonidoDanio, Camera.main.transform.position, volumen);

            CheckpointManager cp = CheckpointManager.Instance;
            if (cp != null)
                cp.Respawn();
            else
            {
                cp = Object.FindFirstObjectByType<CheckpointManager>();
                if (cp != null) cp.Respawn();
            }
        }
    }

    public void ResetVidas()
    {
        vidas = vidasMaximas;

        if (GameManager.Instance != null)
        {
            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            if (sd != null) sd.livesRemaining = vidas;
            GameManager.Instance.currentLives = vidas;
            GameManager.Instance.SaveGame();
        }

        UIManager.Instance?.ActualizarVidas(vidas);
    }
}