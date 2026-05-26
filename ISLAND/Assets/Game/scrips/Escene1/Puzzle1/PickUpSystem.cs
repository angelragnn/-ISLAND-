using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    public Transform carryPoint;
    public float pickUpRange = 3f;
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
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject == null)
                TryPickUp();
            else if (!insideSocket)
                Drop();
        }
    }

    void TryPickUp()
    {
        GameObject[] pickableObjects = GameObject.FindGameObjectsWithTag("Pickable");
        float closestDistance = pickUpRange;
        GameObject closestObject = null;

        foreach (GameObject obj in pickableObjects)
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
            }

            if (heldCollider != null)
                heldCollider.enabled = false;

            heldObject.transform.SetParent(carryPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void Drop()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;
        }

        if (heldCollider != null)
            heldCollider.enabled = true;

        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }

    public void ForceRelease()
    {
        if (heldObject == null) return;
        heldObject.transform.SetParent(null);
        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }
}