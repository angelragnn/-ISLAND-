using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[ZonaCaida] Jugador cayo. Checkpoint en: {CheckpointManager.Instance?.currentCheckpointPosition}");

        LifeManager.Instance?.PerderVida();

        if (CheckpointManager.Instance != null)
        {
            Debug.Log($"[ZonaCaida] Moviendo a: {CheckpointManager.Instance.currentCheckpointPosition}");
            CheckpointManager.Instance.RespawnPlayer();
            Debug.Log($"[ZonaCaida] Posicion jugador despues: {CheckpointManager.Instance.playerTransform?.position}");
        }
    }
}