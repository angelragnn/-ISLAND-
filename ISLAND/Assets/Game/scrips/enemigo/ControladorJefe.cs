using UnityEngine;
using TMPro;

public class ControladorJefe : MonoBehaviour
{
    [Header("Referencias del Jefe")]
    [SerializeField] private GameObject personajeJefe;

    [Header("Interfaz (UI)")]
    [SerializeField] private TextMeshProUGUI textoObjetivoJefe;

    [Header("Configuración")]
    [SerializeField] private int minionsRequeridos = 5;

    private int minionsMuertos = 0;

    void Start()
    {
        if (personajeJefe != null)
        {
            personajeJefe.SetActive(false);
        }

        ActualizarTexto();
    }

    public void RegistrarMinionMuerto()
    {
        minionsMuertos++;
        ActualizarTexto();

        if (minionsMuertos >= minionsRequeridos)
        {
            InvocarJefeFinal();
        }
    }

    void ActualizarTexto()
    {
        if (textoObjetivoJefe != null)
        {
            textoObjetivoJefe.text = $"Esqueletos Élite: {minionsMuertos} / {minionsRequeridos}";
        }
    }

    void InvocarJefeFinal()
    {
        if (personajeJefe != null)
        {
            personajeJefe.SetActive(true);

            if (textoObjetivoJefe != null)
            {
                textoObjetivoJefe.text = "¡EL JEFE FINAL HA DESPERTADO!";
            }
        }
    }
}