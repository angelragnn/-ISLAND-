using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    public Transform playerTransform;
    public Vector3 currentCheckpointPosition;

    void Awake() { Instance = this; }

    void Start()
    {
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
            StartCoroutine(DoRespawn());
    }

    IEnumerator DoRespawn()
    {
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        MovePlayer mp = playerTransform.GetComponent<MovePlayer>();

        if (mp != null) mp.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            yield return null;
            playerTransform.position = currentCheckpointPosition;
            yield return null;
            rb.isKinematic = false;
        }
        else
        {
            yield return null;
            playerTransform.position = currentCheckpointPosition;
        }

        if (mp != null) mp.enabled = true;
    }
}