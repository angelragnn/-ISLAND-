using UnityEngine;

public class Scene1Controller : MonoBehaviour
{
    [Header("Puzzle 1: Recolección (Llaves)")]
    public int keysRequired = 2;            // Número de llaves requeridas para abrir la puerta
    private int keysCollected = 0;
    public DoorController targetDoor;       // La puerta que se abrirá al conseguir las llaves

    [Header("Puzzle 2: Sockets (4 Libros)")]
    public int totalSockets = 4;
    private int socketsActivated = 0;
    
    [Header("Spawn de Recompensa (Llave)")]
    public GameObject keyPrefab;            // El prefab de la llave a aparecer
    public Transform keySpawnPoint;         // El Empty en la escena donde aparecerá la llave
    public AudioClip puzzleCompleteSound;    // Sonido al completar el puzzle de los 4 libros
    
    private AudioSource audioSource;

    void Start()
    {
        // Asegurarnos de tener un AudioSource en este objeto para reproducir el sonido final
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // --- MÉTODOS DEL PUZZLE 1: RECOLECCIÓN DE LLAVES ---
    public void OnKeyCollected()
    {
        keysCollected++;
        Debug.Log($"[Scene1Controller] Llave recolectada: {keysCollected}/{keysRequired}");

        if (keysCollected >= keysRequired)
        {
            OpenPuzzleDoor();
        }
    }

    private void OpenPuzzleDoor()
    {
        if (targetDoor != null)
        {
            targetDoor.ToggleDoor();
            Debug.Log("[Scene1Controller] ¡Puerta del puzzle abierta!");
        }
        else
        {
            Debug.LogWarning("[Scene1Controller] ¡Todas las llaves recolectadas, pero no hay ninguna puerta (Target Door) asignada en el Inspector!");
        }
    }

    // --- MÉTODOS DEL PUZZLE 2: SOCKETS ---
    public void OnSocketActivated()
    {
        socketsActivated++;
        Debug.Log($"[Scene1Controller] Socket de libro activado: {socketsActivated}/{totalSockets}");

        if (socketsActivated >= totalSockets)
        {
            CompleteBookPuzzle();
        }
    }

    private void CompleteBookPuzzle()
    {
        Debug.Log("[Scene1Controller] ¡Puzzle de libros COMPLETADO!");

        // 1. Reproducir sonido de éxito
        if (puzzleCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(puzzleCompleteSound);
        }
        else
        {
            Debug.LogWarning("[Scene1Controller] Sonido de puzzle completado no asignado o falta AudioSource.");
        }

        // 2. Spawnear la llave en el punto vacío (Empty)
        if (keyPrefab != null && keySpawnPoint != null)
        {
            GameObject spawnedKey = Instantiate(keyPrefab, keySpawnPoint.position, keySpawnPoint.rotation);
            Debug.Log($"[Scene1Controller] ¡Llave {spawnedKey.name} aparecida con éxito en {keySpawnPoint.position}!");
        }
        else
        {
            Debug.LogError("[Scene1Controller] No se pudo spawnear la llave. Asegúrate de asignar 'Key Prefab' y 'Key Spawn Point' en el Inspector.");
        }
    }
}
