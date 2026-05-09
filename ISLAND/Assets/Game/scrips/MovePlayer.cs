using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speedplayer = 5.0f;
    public float speedRotation = 200f;

    private float x;
    private float y;
    private Vector2 movementInput;

    public void Update()
    {
        
        x = movementInput.x;
        y = movementInput.y;

        
        transform.Rotate(0, x * speedRotation * Time.deltaTime, 0);

        
        transform.Translate(0, 0, y * speedplayer * Time.deltaTime);
    }

    
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
}