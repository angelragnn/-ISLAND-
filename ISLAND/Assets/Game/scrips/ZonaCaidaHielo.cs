using UnityEngine;

public class ZonaCaidaSimple : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeManager.Instance?.PerderVida();
        CheckpointManager.Instance?.Respawn();
    }
}