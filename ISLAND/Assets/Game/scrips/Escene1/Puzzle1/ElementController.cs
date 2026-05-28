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

        if (_alreadyCollected) return;
        _alreadyCollected = true;


        if (collectParticles != null)
        {
            collectParticles.transform.SetParent(null);
            collectParticles.Play();
        }

        if (collectAudio != null)
            collectAudio.Play();


        if (GameManager.Instance != null)
            GameManager.Instance.RegisterCollection();




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
