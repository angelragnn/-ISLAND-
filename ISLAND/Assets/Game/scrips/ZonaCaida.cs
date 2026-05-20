using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    public Transform puntoSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeManager.Instance.PerderVida();

        // Respawn
        other.transform.position = puntoSpawn.position;
    }
}
