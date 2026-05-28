using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 checkpointPosition;
    private Vector3 initialPosition;
    private bool hasCheckpoint = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            initialPosition = player.transform.position;
            
            if (GameManager.Instance != null && GameManager.Instance.HasSavedCheckpoint())
            {
                checkpointPosition = GameManager.Instance.GetSavedCheckpoint();
                hasCheckpoint = true;
                player.transform.position = checkpointPosition;
                Debug.Log($"[CPM] Checkpoint cargado desde JSON en Start: {checkpointPosition}");
            }
            else
            {
                checkpointPosition = initialPosition;
                hasCheckpoint = false;
                Debug.Log($"[CPM] Usando posicion inicial en Start: {initialPosition}");
            }
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"[CPM] Checkpoint guardado en: {position}");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveCheckpoint(position);
        }
    }

    public void ResetToInitial()
    {
        checkpointPosition = initialPosition;
        Debug.Log($"[CPM] Checkpoint reseteado al inicial: {initialPosition}");
    }

    public void Respawn()
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("[CPM] Sin checkpoint - usando posicion inicial.");
            checkpointPosition = initialPosition;
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
