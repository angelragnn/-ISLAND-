using UnityEngine;

public class Scene1Controller : MonoBehaviour
{
    [Header("Puzzle 1: Recolección (Llaves)")]
    public int keysRequired = 2;
    private int keysCollected = 0;
    private bool doorOpened = false;        // Bandera para que la puerta solo se abra UNA VEZ
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
    }

    // --- PUZZLE 1: LLAVES ---
    public void OnKeyCollected()
    {
        // Protección: si la puerta ya se abrió, ignorar
        if (doorOpened) return;

        keysCollected++;
        Debug.Log($"[Scene1Controller] Llave recolectada: {keysCollected}/{keysRequired}");

        // Solo abre cuando se alcanzan EXACTAMENTE las llaves necesarias
        if (keysCollected == keysRequired)
        {
            OpenPuzzleDoor();
        }
    }

    private void OpenPuzzleDoor()
    {
        // Protección doble: nunca abrir dos veces
        if (doorOpened) return;
        doorOpened = true;

        if (targetDoor != null)
        {
            targetDoor.ToggleDoor();
            Debug.Log("[Scene1Controller] ¡Puerta abierta con éxito!");
        }
        else
        {
            Debug.LogWarning("[Scene1Controller] No hay ninguna puerta (Target Door) asignada en el Inspector!");
        }
    }

    // --- PUZZLE 2: SOCKETS (LIBROS) ---
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
        Debug.Log("[Scene1Controller] ¡Puzzle de libros COMPLETADO!");

        if (puzzleCompleteSound != null && audioSource != null)
            audioSource.PlayOneShot(puzzleCompleteSound);

        if (keyPrefab != null && keySpawnPoint != null)
        {
            Instantiate(keyPrefab, keySpawnPoint.position, keySpawnPoint.rotation);
            Debug.Log($"[Scene1Controller] ¡Llave spawneada en {keySpawnPoint.position}!");
        }
        else
        {
            Debug.LogError("[Scene1Controller] Falta asignar 'Key Prefab' o 'Key Spawn Point' en el Inspector.");
        }
    }
}
