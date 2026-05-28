using UnityEngine;
using TMPro;

public class ResumenVictoria : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoResumenTMP;

    void Start()
    {
        if (textoResumenTMP == null) return;

        int vidasFinales = 5;
        int esqueletosNegros = 0;
        int esqueletosNormales = 0;
        int llaves = 0;
        int gemas = 0;

        if (GameManager.Instance != null)
        {
            vidasFinales = GameManager.Instance.currentLives;

            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            if (sd != null && sd.collectedItems != null)
            {
                foreach (string item in sd.collectedItems)
                {
                    if (item == "EsqueletoNegro") esqueletosNegros++;
                    else if (item == "EsqueletoNormal") esqueletosNormales++;
                    else if (item == "Llave") llaves++;
                    else if (item == "Gema") gemas++;
                }
            }
        }

        textoResumenTMP.text = "RESUMEN DE LA PARTIDA\n\n" +
                               " Vidas restantes: " + vidasFinales + "\n" +
                               " Esqueletos Negros derrotados: " + esqueletosNegros + "\n" +
                               " Esqueletos Normales derrotados: " + esqueletosNormales + "\n" +
                               "Llaves encontradas: " + llaves + "\n" +
                               "Gemas recolectadas: " + gemas + "\n\n" +
                               "¡Gracias por jugar!";
    }
}