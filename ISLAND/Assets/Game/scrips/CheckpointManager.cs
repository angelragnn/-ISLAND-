using UnityEngine;

// ============================================================
//  CHECKPOINT MANAGER � Un solo objeto en la escena
//  Los checkpoints individuales tienen el script "Checkpoint.cs"
// ============================================================
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public Transform playerTransform;
    public Vector3 currentCheckpointPosition;

    void Awake()
    {
        Instance = this;
        // Punto de inicio por defecto
        if (playerTransform != null)
            currentCheckpointPosition = playerTransform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpointPosition = position;
        Debug.Log($"[Checkpoint] Nuevo checkpoint: {position}");
    }

    public void RespawnPlayer()
    {
        if (playerTransform != null)
        {
            // Desactivar fisicas momentaneamente para mover sin problemas
            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            playerTransform.position = currentCheckpointPosition;
        }
    }
}

// ============================================================
//  CHECKPOINT � Poner en cada objeto checkpoint de la escena
// ============================================================
public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            CheckpointManager.Instance?.SetCheckpoint(transform.position);

            // Feedback visual � cambia color del checkpoint
            Renderer r = GetComponent<Renderer>();
            if (r != null) r.material.color = Color.green;
        }
    }
}