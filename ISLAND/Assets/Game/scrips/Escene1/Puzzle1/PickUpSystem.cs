using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public Transform carryPoint;
    public float pickUpRange = 3f;
    public string pickableTag = "Pickable";

    [Header("Estado Actual (Lectura)")]
    public GameObject heldObject;

    private Rigidbody heldRb;
    private Collider heldCollider;
    private bool insideSocket = false;

    public void SetInsideSocket(bool value)
    {
        insideSocket = value;
    }

    void Update()
    {

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
            else if (!insideSocket)
            {
                Drop();
            }
        }
    }

    void LateUpdate()
    {


        if (heldObject != null && carryPoint != null)
        {
            heldObject.transform.position = carryPoint.position;
            heldObject.transform.rotation = carryPoint.rotation;
        }
    }

    void TryPickUp()
    {

        GameObject[] pickables = GameObject.FindGameObjectsWithTag(pickableTag);
        float closestDistance = pickUpRange;
        GameObject closestObject = null;

        foreach (GameObject obj in pickables)
        {

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }


        if (closestObject != null)
        {
            heldObject = closestObject;
            heldRb = heldObject.GetComponent<Rigidbody>();
            heldCollider = heldObject.GetComponent<Collider>();


            if (heldRb != null)
            {
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;
            }


            if (heldCollider != null)
            {
                heldCollider.enabled = false;
            }
        }
    }

    public void Drop()
    {
        if (heldObject == null) return;


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


        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }


    public void ForceRelease()
    {
        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }
}