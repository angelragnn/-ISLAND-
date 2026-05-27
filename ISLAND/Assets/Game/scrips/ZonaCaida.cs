using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        LifeManager.Instance?.PerderVida();
    }
}