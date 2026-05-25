using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float rotationAmount = 90f;
    private bool isOpen = false;

    public void ToggleDoor()
    {
        if (!isOpen)
        {
            transform.Rotate(0, rotationAmount, 0); 
        }
        else
        {
            transform.Rotate(0, -rotationAmount, 0); 
        }
        isOpen = !isOpen;
    }
}