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