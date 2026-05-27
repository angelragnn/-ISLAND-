using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float spawnOffsetY = 1.5f;
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Checkpoint] TriggerEnter con: {other.gameObject.name} tag: {other.tag}");
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            Vector3 spawnPos = transform.position + Vector3.up * spawnOffsetY;
            Debug.Log($"[Checkpoint] Guardando posicion: {spawnPos}");
            CheckpointManager.Instance?.SetCheckpoint(spawnPos);
            Renderer r = GetComponent<Renderer>();
            if (r != null) r.material.color = Color.green;
        }
    }
}