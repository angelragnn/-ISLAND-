using UnityEngine;

public class ElementController : MonoBehaviour
{
    [Header("Identificación")]
    public string gemName = "Gema";

    [Header("Efectos")]
    public ParticleSystem collectParticles;
    public AudioSource collectAudio;

    [Header("Configuración")]
    public float destroyDelay = 1.5f;

    private bool _alreadyCollected = false;

    public void Interact()
    {
        // Si ya fue recolectado, SALIR INMEDIATAMENTE. No se ejecuta nada más.
        if (_alreadyCollected) return;
        _alreadyCollected = true;

        // Efectos visuales y de sonido
        if (collectParticles != null)
        {
            collectParticles.transform.SetParent(null);
            collectParticles.Play();
        }

        if (collectAudio != null)
            collectAudio.Play();

        // Notificar al GameManager (puntos, estadísticas)
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterCollection();

        // Notificar al Scene1Controller para el conteo de llaves.
        // Se hace AQUÍ dentro para garantizar que solo se llama UNA VEZ
        // gracias a la protección de _alreadyCollected arriba.
        Scene1Controller sc = FindFirstObjectByType<Scene1Controller>();
        if (sc != null)
            sc.OnKeyCollected();

        Invoke(nameof(DestroyObject), destroyDelay);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
