using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PuertaPuzzle : MonoBehaviour
{
    [Header("Configuracion")]
    public float rangoDeteccion = 3f;
    public Transform jugador;
    public GameObject puertaPadre;

    [Header("UI")]
    public GameObject panelInteraccion;
    public GameObject panelPuzzle;
    public TextMeshProUGUI textoFeedback;

    [Header("Botones")]
    public Button boton1;
    public Button boton2;
    public Button boton3;

    private readonly string[] ordenCorrecto = { "Rojo", "Azul", "Amarillo" };
    private readonly Color[] colores = {
        new Color(0.9f, 0.15f, 0.15f),
        new Color(0.15f, 0.4f, 0.9f),
        new Color(0.95f, 0.85f, 0.1f)
    };
    private readonly string[] nombresColores = { "Rojo", "Azul", "Amarillo" };

    private string[] asignacionBotones = new string[3];
    private List<string> secuenciaJugador = new List<string>();

    private bool cerca = false;
    private bool puzzleActivo = false;
    private bool puertaAbierta = false;
    private MovePlayer movePlayer;
    private string uniqueID;

    void Start()
    {
        uniqueID = $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_{gameObject.name}";

        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                jugador = p.transform;
                movePlayer = p.GetComponent<MovePlayer>();
            }
        }
        else
        {
            movePlayer = jugador.GetComponent<MovePlayer>();
        }

        if (GameManager.Instance != null && GameManager.Instance.IsDoorOpened(uniqueID))
        {
            puertaAbierta = true;
            if (panelInteraccion != null) panelInteraccion.SetActive(false);
            if (panelPuzzle != null) panelPuzzle.SetActive(false);
            if (puertaPadre != null) puertaPadre.SetActive(false);
            return;
        }

        panelInteraccion.SetActive(false);
        panelPuzzle.SetActive(false);

        boton1.onClick.AddListener(() => PulsarBoton(1));
        boton2.onClick.AddListener(() => PulsarBoton(2));
        boton3.onClick.AddListener(() => PulsarBoton(3));
    }

    void Update()
    {
        if (puertaAbierta) return;
        if (jugador == null) return;

        float dist = Vector3.Distance(transform.position, jugador.position);
        cerca = dist <= rangoDeteccion;

        if (!puzzleActivo)
            panelInteraccion.SetActive(cerca);

        if (cerca && !puzzleActivo && Keyboard.current.eKey.wasPressedThisFrame)
            AbrirPuzzle();

        if (puzzleActivo && Keyboard.current.escapeKey.wasPressedThisFrame)
            CerrarPuzzle();
    }

    void AbrirPuzzle()
    {
        puzzleActivo = true;
        panelInteraccion.SetActive(false);
        panelPuzzle.SetActive(true);
        secuenciaJugador.Clear();
        textoFeedback.text = "";
        AsignarColoresAleatorios();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (movePlayer != null) movePlayer.enabled = false;
    }

    void CerrarPuzzle()
    {
        puzzleActivo = false;
        panelPuzzle.SetActive(false);
        secuenciaJugador.Clear();
        textoFeedback.text = "";

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (movePlayer != null) movePlayer.enabled = true;
    }

    void AsignarColoresAleatorios()
    {
        List<int> indices = new List<int> { 0, 1, 2 };
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = indices[i]; indices[i] = indices[r]; indices[r] = tmp;
        }

        Button[] botones = { boton1, boton2, boton3 };
        for (int i = 0; i < 3; i++)
        {
            int colorIdx = indices[i];
            asignacionBotones[i] = nombresColores[colorIdx];

            ColorBlock cb = botones[i].colors;
            cb.normalColor = colores[colorIdx];
            cb.highlightedColor = colores[colorIdx] * 1.2f;
            cb.pressedColor = colores[colorIdx] * 0.8f;
            cb.selectedColor = colores[colorIdx];
            botones[i].colors = cb;

            TextMeshProUGUI txt = botones[i].GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = nombresColores[colorIdx];
        }
    }

    public void PulsarBoton(int numBoton)
    {
        if (!puzzleActivo) return;

        string colorPulsado = asignacionBotones[numBoton - 1];
        secuenciaJugador.Add(colorPulsado);

        int paso = secuenciaJugador.Count - 1;

        if (secuenciaJugador[paso] != ordenCorrecto[paso])
        {
            StartCoroutine(MostrarFeedback("Orden incorrecto. Intenta de nuevo.", Color.red));
            secuenciaJugador.Clear();
            return;
        }

        if (secuenciaJugador.Count == ordenCorrecto.Length)
            StartCoroutine(PuzzleCompletado());
        else
        {
            textoFeedback.text = $"Correcto... {secuenciaJugador.Count}/{ordenCorrecto.Length}";
            textoFeedback.color = Color.green;
        }
    }

    IEnumerator PuzzleCompletado()
    {
        puertaAbierta = true;
        textoFeedback.text = "Puerta abierta!";
        textoFeedback.color = Color.green;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordDoorOpened(uniqueID);
        }

        yield return new WaitForSeconds(1f);

        panelPuzzle.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (movePlayer != null) movePlayer.enabled = true;

        if (puertaPadre != null)
            puertaPadre.SetActive(false);
    }

    IEnumerator MostrarFeedback(string mensaje, Color color)
    {
        textoFeedback.text = mensaje;
        textoFeedback.color = color;
        yield return new WaitForSeconds(1.5f);
        textoFeedback.text = "";
        AsignarColoresAleatorios();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
