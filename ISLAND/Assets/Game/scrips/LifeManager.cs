using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoGameOver;
    [SerializeField][Range(0f, 1f)] private float volumen = 0.4f;

    private int vidas = 3;
    public int Vidas => vidas;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PerderVida()
    {
        if (vidas <= 0) return;

        vidas--;
        UIManager.Instance.ActualizarVidas(vidas);

        if (vidas <= 0)
        {
            AudioSource.PlayClipAtPoint(sonidoGameOver, Camera.main.transform.position, volumen);
            UIManager.Instance.MostrarGameOver();
        }
        else
        {
            AudioSource.PlayClipAtPoint(sonidoDanio, Camera.main.transform.position, volumen);
        }
    }
}