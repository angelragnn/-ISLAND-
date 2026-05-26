using UnityEngine;

public class Scene1Controller : MonoBehaviour
{
    [Header("Configuración del Puzzle")]
    public int gemsRequired = 3;
    private int gemsCollected = 0;

    [Header("Referencias")]
    public DoorController targetDoor;

    public void OnGemCollected()
    {
        gemsCollected++;
        Debug.Log($"[Scene1Controller] Gema recolectada: {gemsCollected}/{gemsRequired}");

        if (gemsCollected >= gemsRequired)
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
            Debug.LogWarning("[Scene1Controller] ¡Todas las gemas recolectadas, pero no hay ninguna puerta (DoorController) asignada en el Inspector!");
        }
    }
}
