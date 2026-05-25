using UnityEngine;

public class PlataformaCae : MonoBehaviour
{
    public float tiempoAntesDeCaer = 1f;
    private bool pisada = false;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && !pisada)
        {
            pisada = true;
            Invoke("Caer", tiempoAntesDeCaer);
        }
    }

    void Caer()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        Destroy(gameObject, 3f);
    }
}