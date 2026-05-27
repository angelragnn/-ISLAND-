using UnityEngine;
using System.Collections;

public class PlataformaCae : MonoBehaviour
{
    public float tiempoAntesDeCaer = 1.5f;
    public float velocidadSacudida = 25f;
    public float amplitudSacudida = 0.05f;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Rigidbody rb;
    private bool pisada = false;
    private bool cayendo = false;
    private Coroutine rutinaActual;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied += Resetear;
            Debug.Log($"[PlataformaCae] {gameObject.name} suscrita al evento OnPlayerDied");
        }
        else
        {
            Debug.LogWarning($"[PlataformaCae] {gameObject.name} NO encontro GameManager en Start");
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerDied -= Resetear;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && !pisada && !cayendo)
        {
            pisada = true;
            rutinaActual = StartCoroutine(SacudirYCaer());
        }
    }

    IEnumerator SacudirYCaer()
    {
        float tiempoRestante = tiempoAntesDeCaer;

        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            float offset = Mathf.Sin(Time.time * velocidadSacudida) * amplitudSacudida;
            transform.position = posicionOriginal + new Vector3(offset, 0f, offset * 0.5f);
            yield return null;
        }

        cayendo = true;
        transform.position = posicionOriginal;
        rb.isKinematic = false;
    }

    public void Resetear()
    {
        Debug.Log($"[PlataformaCae] {gameObject.name} recibio Resetear()");

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;

        pisada = false;
        cayendo = false;

        gameObject.SetActive(true);
    }
}