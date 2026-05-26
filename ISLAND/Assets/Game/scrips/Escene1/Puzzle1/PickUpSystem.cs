using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public Transform carryPoint;           // El punto (hijo del jugador) donde flotará el objeto
    public float pickUpRange = 3f;         // Rango de distancia para agarrar objetos
    public string pickableTag = "Pickable"; // Tag de los objetos que se pueden agarrar

    [Header("Estado Actual (Lectura)")]
    public GameObject heldObject;          // Objeto actualmente agarrado

    private Rigidbody heldRb;
    private Collider heldCollider;

    void Update()
    {
        // Detectar pulsación de la tecla E usando el nuevo Input System
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
            else
            {
                Drop();
            }
        }
    }

    void LateUpdate()
    {
        // En LateUpdate movemos el objeto para que siga al carryPoint después de que el jugador se mueva.
        // Esto evita temblores (jittering) y desfases de posición.
        if (heldObject != null && carryPoint != null)
        {
            heldObject.transform.position = carryPoint.position;
            heldObject.transform.rotation = carryPoint.rotation;
        }
    }

    void TryPickUp()
    {
        // 1. Buscar todos los objetos con el tag correspondiente
        GameObject[] pickables = GameObject.FindGameObjectsWithTag(pickableTag);
        float closestDistance = pickUpRange;
        GameObject closestObject = null;

        foreach (GameObject obj in pickables)
        {
            // Medir la distancia desde la posición de este script (el jugador) al objeto
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        // 2. Si encontramos un objeto válido dentro del rango
        if (closestObject != null)
        {
            heldObject = closestObject;
            heldRb = heldObject.GetComponent<Rigidbody>();
            heldCollider = heldObject.GetComponent<Collider>();

            // Desactivar físicas para que el objeto flote y no pese
            if (heldRb != null)
            {
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;
            }

            // Desactivar colisiones para que el objeto no choque con el jugador al caminar
            if (heldCollider != null)
            {
                heldCollider.enabled = false;
            }
        }
    }

    public void Drop()
    {
        if (heldObject == null) return;

        // Reactivar colisiones y físicas
        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;
            heldRb.linearVelocity = Vector3.zero;
            heldRb.angularVelocity = Vector3.zero;
        }

        if (heldCollider != null)
        {
            heldCollider.enabled = true;
        }

        // Liberar referencias sin alterar la jerarquía (evita problemas de escala y herencia)
        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }

    // Método por si necesitas soltar el objeto desde otros scripts (ej: recibir daño)
    public void ForceRelease()
    {
        Drop();
    }
}