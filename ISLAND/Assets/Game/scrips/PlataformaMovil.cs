using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [Header("Movimiento")]
    public float distancia = 3f;
    public float velocidad = 2f;
    public Vector3 direccion = Vector3.forward;

    private Vector3 puntoA;
    private Vector3 puntoB;
    private Vector3 posicionAnterior;
    private Transform jugadorEncima;

    void Start()
    {
        puntoA = transform.position;
        puntoB = transform.position + direccion.normalized * distancia;
        posicionAnterior = transform.position;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;
        transform.position = Vector3.Lerp(puntoA, puntoB, t);

        // Mover al jugador la misma diferencia que se movio la plataforma
        Vector3 delta = transform.position - posicionAnterior;
        if (jugadorEncima != null)
            jugadorEncima.position += delta;

        posicionAnterior = transform.position;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
            jugadorEncima = col.transform;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
            jugadorEncima = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 b = transform.position + direccion.normalized * distancia;
        Gizmos.DrawLine(transform.position, b);
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.DrawWireSphere(b, 0.2f);
    }
}