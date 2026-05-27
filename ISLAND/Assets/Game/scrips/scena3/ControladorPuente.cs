using UnityEngine;
using TMPro; 

public class ControladorPuente : MonoBehaviour
{
    [Header("Referencias del Mapa")]
    [Tooltip("Arrastra aquí el objeto del puente que está sobre la lava")]
    [SerializeField] private GameObject puente;

    [Header("Referencias de Interfaz (UI)")]
    [Tooltip("Arrastra aquí el objeto de Texto (TextMeshPro) que mostrará el contador")]
    [SerializeField] private TextMeshProUGUI textoContador;

    [Header("Configuración de Victoria")]
    [Tooltip("Cantidad de enemigos totales que debes derrotar para activar el puente")]
    [SerializeField] private int enemigosRequeridos = 5;

    private int enemigosDerrotadosActualmente = 0;

    void Start()
    {
        
        if (puente != null)
        {
            puente.SetActive(false);
        }

        
        ActualizarTextoInterfaz();
    }

    public void RegistrarEnemigoMuerto()
    {
        enemigosDerrotadosActualmente++;

        
        ActualizarTextoInterfaz();

        if (enemigosDerrotadosActualmente >= enemigosRequeridos)
        {
            ActivarPuente();
        }
    }

    private void ActualizarTextoInterfaz()
    {
        if (textoContador != null)
        {
            textoContador.text = $"Esqueletos: {enemigosDerrotadosActualmente} / {enemigosRequeridos}";
        }
    }

    private void ActivarPuente()
    {
        if (puente != null)
        {
            puente.SetActive(true);

            if (textoContador != null)
            {
                textoContador.text = "¡Puente Desbloqueado!";
            }

            Debug.Log("¡Puente activado de forma permanente!");
        }
    }
}