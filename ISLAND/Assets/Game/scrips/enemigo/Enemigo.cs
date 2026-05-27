using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;

    [Header("Configuración de Objetivo")]
    public GameObject target;
    public bool atacando;

    private float tiempoSiguienteAtaque = 0f;
    [Header("Ajustes de Combate")]
    public float tiempoEntreAtaques = 0.5f;
    public int vida = 3;
    private int vidaMaxima;

    [Header("Visual - Cambio de Color")]
    [SerializeField] private List<Renderer> todasLasMallas = new List<Renderer>();
    [SerializeField] private Color colorDanoClaro = new Color(1f, 0.5f, 0.5f);
    [SerializeField] private Color colorDanoOscuro = new Color(0.6f, 0f, 0f);

    [Header("Sonidos del Enemigo (3D)")]
    [SerializeField] private AudioClip sonidoRecibirGolpe;
    [SerializeField] private AudioClip sonidoMorir;
    [SerializeField][Range(0f, 1f)] private float volumenSonido = 0.6f;

    [Header("Físicas de Suelo")]
    public LayerMask capaSuelo;
    private float velocidadVertical = 0f;
    private float gravity = 9.81f;

    void Start()
    {
        ani = GetComponent<Animator>();
        ani.SetBool("walk", false);
        ani.SetBool("run", false);
        ani.SetBool("attack", false);
        atacando = false;

        vidaMaxima = vida;

        if (todasLasMallas.Count == 0)
        {
            Renderer[] renderersEncontrados = GetComponentsInChildren<Renderer>();
            todasLasMallas.AddRange(renderersEncontrados);
        }
    }

    void Update()
    {
        if (target == null) return;

        AplicarGravedadYSuelo();
        Comportamiento_Enemigo();
    }

    void AplicarGravedadYSuelo()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f, capaSuelo))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
            velocidadVertical = 0f;
        }
        else
        {
            velocidadVertical -= gravity * Time.deltaTime;
            transform.Translate(Vector3.up * velocidadVertical * Time.deltaTime, Space.World);
        }
    }

    public void Comportamiento_Enemigo()
    {
        float distancia = Vector3.Distance(transform.position, target.transform.position);

        if (distancia > 5)
        {
            ani.SetBool("run", false);
            ani.SetBool("attack", false);
            atacando = false;

            cronometro += Time.deltaTime;
            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    ani.SetBool("walk", false);
                    break;
                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    ani.SetBool("walk", true);
                    break;
            }
        }
        else
        {
            if (distancia > 1.2f)
            {
                if (atacando)
                {
                    ani.SetBool("attack", false);
                    atacando = false;
                }

                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);

                ani.SetBool("walk", false);
                ani.SetBool("run", true);

                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            }
            else
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", false);

                if (Time.time >= tiempoSiguienteAtaque && !atacando)
                {
                    ani.SetBool("attack", true);
                    atacando = true;
                }
            }
        }
    }

    public void Final_Ani()
    {
        ani.SetBool("attack", false);
        atacando = false;
        tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
    }

    public void HacerDanoAlJugador()
    {
        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.PerderVida();
        }
    }

    public void RecibirDano(int cantidad)
    {
        vida -= cantidad;

        ActualizarColorPorDano();

        if (vida <= 0)
        {
            if (sonidoMorir != null)
            {
                AudioSource.PlayClipAtPoint(sonidoMorir, transform.position, volumenSonido);
            }
            Morir();
        }
        else
        {
            if (sonidoRecibirGolpe != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecibirGolpe, transform.position, volumenSonido);
            }
        }
    }

    void ActualizarColorPorDano()
    {
        foreach (Renderer malla in todasLasMallas)
        {
            if (malla == null) continue;

            if (vida == 2)
            {
                malla.material.color = colorDanoClaro;
            }
            else if (vida == 1)
            {
                malla.material.color = colorDanoOscuro;
            }
        }
    }

    void Morir()
    {
        ControladorPuente controlador = Object.FindFirstObjectByType<ControladorPuente>();
        if (controlador != null)
        {
            controlador.RegistrarEnemigoMuerto();
        }

        Destroy(gameObject);
    }
}