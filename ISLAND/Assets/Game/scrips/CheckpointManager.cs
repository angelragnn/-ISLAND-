using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 checkpointPosition;
    private bool hasCheckpoint = false;

    void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"[CPM] Checkpoint guardado en: {position}");
    }

    public void Respawn()
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("[CPM] No hay checkpoint guardado aun.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[CPM] No se encontro el Player.");
            return;
        }

        StartCoroutine(DoRespawn(player));
    }

    IEnumerator DoRespawn(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MonoBehaviour move = player.GetComponent<MovePlayer>();

        if (move != null) move.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        yield return null;
        player.transform.position = checkpointPosition;
        yield return null;

        if (rb != null) rb.isKinematic = false;
        if (move != null) move.enabled = true;

        Debug.Log($"[CPM] Respawn en: {player.transform.position}");
    }
}