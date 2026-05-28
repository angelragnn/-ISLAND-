using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menu Principal")]
    public GameObject panelInstrucciones;

    [Header("In-Game")]
    [SerializeField] private TextMeshProUGUI textoVidas;
    [SerializeField] private GameObject panelGameOver;

    [Header("Contador de Llaves")]
    [SerializeField] private TextMeshProUGUI textoLlaves;
    [SerializeField] private int totalLlavesRequeridas = 2; // Ajusta esto al mismo valor que keysRequired en Scene1Controller

    private int llavesActuales = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    private void Start()
    {
        if (LifeManager.Instance != null && textoVidas != null)
            ActualizarVidas(LifeManager.Instance.Vidas);

        // Suscribirse al evento del GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblePickedUp += OnLlaveRecolectada;

        // Mostrar el estado inicial: "Llave 0/2"
        ActualizarTextoLlaves();
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblePickedUp -= OnLlaveRecolectada;
    }

    // Este método se llama automáticamente cada vez que se recoge un coleccionable
    private void OnLlaveRecolectada(int recolectadas, int total)
    {
        llavesActuales = recolectadas;
        ActualizarTextoLlaves();
    }

    private void ActualizarTextoLlaves()
    {
        if (textoLlaves != null)
            textoLlaves.text = $"Llave {llavesActuales}/{totalLlavesRequeridas}";
    }

    // Método público por si necesitas actualizar las llaves desde otro script
    public void SetLlaves(int actuales, int total)
    {
        llavesActuales = actuales;
        totalLlavesRequeridas = total;
        ActualizarTextoLlaves();
    }

    public void Jugar()
    {
        SceneManager.LoadScene("2BOSQUE");
    }

    public void Instrucciones()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void ActualizarVidas(int vidas)
    {
        if (textoVidas != null)
            textoVidas.text = vidas.ToString();
    }

    public void MostrarGameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}