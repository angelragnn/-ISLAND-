using UnityEngine;
using System.Collections;

public class PlataformaCae : MonoBehaviour
{
    [Header("Configuracion")]
    public float tiempoAntesDeCaer = 10f;
    public float tiempoHastaDestruir = 3f;

    private bool pisada = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && !pisada)
        {
            pisada = true;
            StartCoroutine(ContarYCaer());
        }
    }

    IEnumerator ContarYCaer()
    {
        float tiempoRestante = tiempoAntesDeCaer;

        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        rb.isKinematic = false;
        Destroy(gameObject, tiempoHastaDestruir);
    }
}