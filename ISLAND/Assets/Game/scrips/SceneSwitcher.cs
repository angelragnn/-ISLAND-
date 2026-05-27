using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad;
    public float duracionFade = 2f;
    private bool estaCargando = false;

    private void OnTriggerEnter(Collider other)
    {
        if (estaCargando) return;

        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                StartCoroutine(SecuenciaCambio());
            }
            else
            {
                Debug.LogWarning("No has escrito el nombre de la escena en el Inspector.");
            }
        }
    }

    private IEnumerator SecuenciaCambio()
    {
        estaCargando = true;

        if (FadeController.Instance != null)
        {
            yield return StartCoroutine(FadeController.Instance.FadeIn(duracionFade));
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}