using UnityEngine;

public class RecolectableActivador : MonoBehaviour
{
    [Header("Camino alternativo")]
    public GameObject[] plataformasAlternativas;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            foreach (GameObject p in plataformasAlternativas)
                p.SetActive(true);

            
            GameManager.Instance?.OnCollectibleCollected(10);

            Destroy(gameObject);
        }
    }
}