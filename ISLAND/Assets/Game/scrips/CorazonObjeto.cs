using UnityEngine;

public class CorazonObjeto : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoRecoleccion;
    [SerializeField][Range(0f, 1f)] private float volumen = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LifeManager.Instance != null)
            {
                if (LifeManager.Instance.Vidas < 5)
                {
                    LifeManager.Instance.RecolectarCorazon();

                    if (sonidoRecoleccion != null)
                    {
                        AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position, volumen);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}