using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsqueletoNegro : MonoBehaviour
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
    public int vida = 5;

    [Header("Visual - Cambio de Color")]
    [SerializeField] private List<Renderer> todasLasMallas = new List<Renderer>();
    [SerializeField] private Color colorInicialNegro = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color colorFase1 = new Color(0.5f, 0.1f, 0.1f);
    [SerializeField] private Color colorFase2 = new Color(0.7f, 0f, 0f);
    [SerializeField] private Color colorFase3 = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color colorFase4 = new Color(1f, 0.6f, 0.6f);

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


        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }

        if (todasLasMallas.Count == 0)
        {
            Renderer[] renderersEncontrados = GetComponentsInChildren<Renderer>();
            todasLasMallas.AddRange(renderersEncontrados);
        }

        PintarEsqueletoCompleto(colorInicialNegro);
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
                    transform.Translate(Vector3.forward * 1.8f * Time.deltaTime);
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

                transform.Translate(Vector3.forward * 3.5f * Time.deltaTime);
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
                AudioSource.PlayClipAtPoint(sonidoMorir, transform.position, volumenSound);
            }
            void Morir()
            {
                ControladorJefe controlador = Object.FindFirstObjectByType<ControladorJefe>();
                if (controlador != null)
                {
                    controlador.RegistrarMinionMuerto();
                }

                Destroy(gameObject);
            }
            Morir();
        }
        else
        {
            if (sonidoRecibirGolpe != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecibirGolpe, transform.position, volumenSound);
            }
        }
    }

    private float volumenSound { get { return volumenSonido; } }

    void ActualizarColorPorDano()
    {
        if (vida == 4) PintarEsqueletoCompleto(colorFase1);
        else if (vida == 3) PintarEsqueletoCompleto(colorFase2);
        else if (vida == 2) PintarEsqueletoCompleto(colorFase3);
        else if (vida == 1) PintarEsqueletoCompleto(colorFase4);
    }

    void PintarEsqueletoCompleto(Color nuevoColor)
    {
        foreach (Renderer malla in todasLasMallas)
        {
            if (malla != null)
            {
                malla.material.color = nuevoColor;
            }
        }
    }

    void Morir()
    {
        ControladorJefe controlador = Object.FindFirstObjectByType<ControladorJefe>();
        if (controlador != null)
        {
            controlador.RegistrarMinionMuerto();
        }

        Destroy(gameObject);
    }
}