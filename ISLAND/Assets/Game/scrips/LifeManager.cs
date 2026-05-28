using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoGameOver;
    [SerializeField][Range(0f, 1f)] private float volumen = 0.4f;
    [SerializeField] private int vidasMaximas = 5;

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
            vidas = GameManager.Instance.currentLives;
        }
        else
        {
            if (PlayerPrefs.HasKey("VidasGuardadasGlobal"))
            {
                vidas = PlayerPrefs.GetInt("VidasGuardadasGlobal");
            }
            else
            {
                vidas = vidasMaximas;
            }
        }

        UIManager.Instance?.ActualizarVidas(vidas);
    }

    public void PerderVida()
    {
        if (vidas <= 0) return;
        vidas--;

        ActualizarPersistencia();

        UIManager.Instance?.ActualizarVidas(vidas);

        if (vidas <= 0)
        {
            AudioSource.PlayClipAtPoint(sonidoGameOver, Camera.main.transform.position, volumen);
            UIManager.Instance?.MostrarGameOver();
        }
        else
        {
            AudioSource.PlayClipAtPoint(sonidoDanio, Camera.main.transform.position, volumen);
        }
    }

    public void RecolectarCorazon()
    {
        if (vidas >= vidasMaximas) return;
        vidas++;

        ActualizarPersistencia();

        UIManager.Instance?.ActualizarVidas(vidas);
    }

    private void ActualizarPersistencia()
    {
        PlayerPrefs.SetInt("VidasGuardadasGlobal", vidas);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLives = vidas;
            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            if (sd != null)
            {
                sd.livesRemaining = vidas;
            }
            GameManager.Instance.SaveGame();
        }
    }

    public void ResetVidas()
    {
        vidas = vidasMaximas;
        ActualizarPersistencia();
        UIManager.Instance?.ActualizarVidas(vidas);
    }
}