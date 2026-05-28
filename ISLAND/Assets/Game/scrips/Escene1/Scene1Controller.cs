using UnityEngine;

public class Scene1Controller : MonoBehaviour
{
    [Header("Puzzle 1: Recoleccion (Llaves)")]
    public int keysRequired = 2;
    private int keysCollected = 0;
    private bool doorOpened = false;
    public DoorController targetDoor;

    [Header("Puzzle 2: Sockets (4 Libros)")]
    public int totalSockets = 4;
    private int socketsActivated = 0;

    [Header("Spawn de Recompensa (Llave)")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    public AudioClip puzzleCompleteSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (GameManager.Instance != null)
        {
            SceneData sd = GameManager.Instance.GetCurrentSceneData();
            if (sd != null)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                keysCollected = 0;
                if (sd.collectedItems != null)
                {
                    foreach (string id in sd.collectedItems)
                    {
                        if (id.StartsWith(sceneName))
                        {
                            keysCollected++;
                        }
                    }
                }

                socketsActivated = 0;
                if (sd.activatedSockets != null)
                {
                    foreach (string id in sd.activatedSockets)
                    {
                        if (id.StartsWith(sceneName))
                        {
                            socketsActivated++;
                        }
                    }
                }

                if (keysCollected >= keysRequired)
                {
                    doorOpened = true;
                }
            }
        }

        ActualizarUILlaves();
    }

    public void OnKeyCollected()
    {
        if (doorOpened) return;

        keysCollected++;
        Debug.Log($"[Scene1Controller] Llave recolectada: {keysCollected}/{keysRequired}");

        ActualizarUILlaves();

        if (keysCollected == keysRequired)
        {
            OpenPuzzleDoor();
        }
    }

    private void ActualizarUILlaves()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetLlaves(keysCollected, keysRequired);
        else
            Debug.LogWarning("[Scene1Controller] UIManager.Instance es null, no se pudo actualizar el texto de llaves.");
    }

    private void OpenPuzzleDoor()
    {
        if (doorOpened) return;
        doorOpened = true;

        if (targetDoor != null)
        {
            targetDoor.ToggleDoor();
            Debug.Log("[Scene1Controller] Puerta abierta con exito!");
        }
        else
        {
            Debug.LogWarning("[Scene1Controller] No hay ninguna puerta (Target Door) asignada en el Inspector!");
        }
    }

    public void OnSocketActivated()
    {
        socketsActivated++;
        Debug.Log($"[Scene1Controller] Socket activado: {socketsActivated}/{totalSockets}");

        if (socketsActivated >= totalSockets)
        {
            CompleteBookPuzzle();
        }
    }

    private void CompleteBookPuzzle()
    {
        Debug.Log("[Scene1Controller] Puzzle de libros COMPLETADO!");

        if (puzzleCompleteSound != null && audioSource != null)
            audioSource.PlayOneShot(puzzleCompleteSound);

        if (keyPrefab != null && keySpawnPoint != null)
        {
            Instantiate(keyPrefab, keySpawnPoint.position, keySpawnPoint.rotation);
            Debug.Log($"[Scene1Controller] Llave spawneada en {keySpawnPoint.position}!");
        }
        else
        {
            Debug.LogError("[Scene1Controller] Falta asignar 'Key Prefab' o 'Key Spawn Point' en el Inspector.");
        }
    }
}
