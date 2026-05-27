using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    private Image imagenFade;
    private float duracionFade = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CrearImagen();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CrearImagen()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(transform, false);
        imagenFade = imgObj.AddComponent<Image>();
        imagenFade.color = new Color(0, 0, 0, 0);

        RectTransform rect = imgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeOut(duracionFade));
    }

    public IEnumerator FadeIn(float duracion)
    {
        duracionFade = duracion;
        float elapsed = 0f;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            imagenFade.color = new Color(0, 0, 0, Mathf.Clamp01(elapsed / duracion));
            yield return null;
        }
        imagenFade.color = new Color(0, 0, 0, 1f);
    }

    private IEnumerator FadeOut(float duracion)
    {
        float elapsed = 0f;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            imagenFade.color = new Color(0, 0, 0, Mathf.Clamp01(1f - elapsed / duracion));
            yield return null;
        }
        imagenFade.color = new Color(0, 0, 0, 0f);
    }
}