using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JefeFinal : MonoBehaviour
{
    private Animator ani;
    private AudioSource fuenteAudio;
    private float tiempoSiguienteAtaque = 0f;
    private float cronometroRutina;
    private int rutina;
    private Quaternion angulo;
    private float grado;
    private bool atacando;
    private bool estaMuerto = false;
    private bool jefeDetectado = false;

    [Header("Configuración de Objetivo")]
    public GameObject target;

    [Header("Ajustes de Combate y Visión")]
    public int vida = 12;
    public float tiempoEntreAtaques = 2f;
    public float rangoParaAtacar = 2.5f;
    public float rangoDeVision = 30f;

    [Header("Interfaz de Usuario (UI)")]
    [SerializeField] private GameObject panelAlertaJefe;

    [Header("Efectos de Sonido (Audios)")]
    [SerializeField] private AudioClip sonidoAparicionJefe;
    [SerializeField] private AudioClip sonidoRecibirDano;
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Visual - Cambio de Color")]
    [SerializeField] private List<Renderer> todasLasMallas = new List<Renderer>();
    [SerializeField] private Color colorDano = new Color(1f, 0.3f, 0.3f);
    private List<Color> coloresOriginales = new List<Color>();

    [Header("Físicas de Suelo")]
    public LayerMask capaSuelo;
    private float velocidadVertical = 0f;
    private float gravity = 9.81f;

    void Start()
    {
        ani = GetComponent<Animator>();
        fuenteAudio = GetComponent<AudioSource>();

        if (fuenteAudio == null)
        {
            fuenteAudio = gameObject.AddComponent<AudioSource>();
        }

        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }

        if (todasLasMallas.Count == 0)
        {
            todasLasMallas.AddRange(GetComponentsInChildren<Renderer>());
        }

        foreach (Renderer malla in todasLasMallas)
        {
            if (malla != null) coloresOriginales.Add(malla.material.color);
        }

        if (panelAlertaJefe != null)
        {
            panelAlertaJefe.SetActive(false);
        }
    }

    void Update()
    {
        if (estaMuerto || target == null) return;

        AplicarGravedadYSuelo();
        ComportamientoJefe();
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

    void ComportamientoJefe()
    {
        Vector3 posicionJefePlana = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 posicionTargetPlana = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        float distanciaHorizontal = Vector3.Distance(posicionJefePlana, posicionTargetPlana);

        if (distanciaHorizontal > rangoDeVision)
        {
            ani.SetBool("walk", false);
            atacando = false;

            cronometroRutina += Time.deltaTime;
            if (cronometroRutina >= 3f)
            {
                rutina = Random.Range(0, 2);
                cronometroRutina = 0;
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
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 1f);
                    transform.Translate(Vector3.forward * 1.5f * Time.deltaTime);
                    ani.SetBool("walk", true);
                    break;
            }
        }
        else
        {
            if (!jefeDetectado)
            {
                jefeDetectado = true;
                ActivarSecuenciaAparicion();
            }

            if (distanciaHorizontal > rangoParaAtacar)
            {
                if (atacando && ani.GetCurrentAnimatorStateInfo(0).IsName("Idle")) { atacando = false; }

                ani.SetBool("walk", true);
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 3f);

                transform.Translate(Vector3.forward * 3f * Time.deltaTime);
            }
            else
            {
                ani.SetBool("walk", false);

                if (Time.time >= tiempoSiguienteAtaque && !atacando)
                {
                    atacando = true;
                    int tipoAtaque = Random.Range(0, 2);
                    if (tipoAtaque == 0)
                    {
                        ani.SetTrigger("Attack01");
                    }
                    else
                    {
                        ani.SetTrigger("Attack02");
                    }
                }
            }
        }
    }

    void ActivarSecuenciaAparicion()
    {
        if (fuenteAudio != null && sonidoAparicionJefe != null)
        {
            fuenteAudio.PlayOneShot(sonidoAparicionJefe);
        }

        if (panelAlertaJefe != null)
        {
            StartCoroutine(EfectoPanelAlerta());
        }
    }

    IEnumerator EfectoPanelAlerta()
    {
        panelAlertaJefe.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            panelAlertaJefe.SetActive(true);
            yield return new WaitForSeconds(0.20f);
            panelAlertaJefe.SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }

        panelAlertaJefe.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        panelAlertaJefe.SetActive(false);
    }

    public void GolpeFisicoAttack01()
    {
        ProcesarDanoImpacto(1);
    }

    public void GolpeFisicoAttack02()
    {
        ProcesarDanoImpacto(2);
    }

    private void ProcesarDanoImpacto(int cantidadCorazones)
    {
        if (estaMuerto || target == null) return;

        Vector3 posicionJefePlana = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 posicionTargetPlana = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        float distanciaReal = Vector3.Distance(posicionJefePlana, posicionTargetPlana);

        if (distanciaReal <= rangoParaAtacar + 0.4f)
        {
            if (LifeManager.Instance != null)
            {
                for (int i = 0; i < cantidadCorazones; i++)
                {
                    LifeManager.Instance.PerderVida();
                }
            }
        }

        atacando = false;
        tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
    }

    public void RecibirDano(int cantidad)
    {
        if (estaMuerto) return;

        vida -= cantidad;
        StartCoroutine(EfectoColorDano());

        if (fuenteAudio != null && sonidoRecibirDano != null && vida > 0)
        {
            fuenteAudio.PlayOneShot(sonidoRecibirDano);
        }

        if (vida <= 0)
        {
            Morir();
        }
    }

    IEnumerator EfectoColorDano()
    {
        foreach (Renderer malla in todasLasMallas)
        {
            if (malla != null) malla.material.color = colorDano;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < todasLasMallas.Count; i++)
        {
            if (todasLasMallas[i] != null && i < coloresOriginales.Count)
            {
                todasLasMallas[i].material.color = coloresOriginales[i];
            }
        }
    }

    void Morir()
    {
        estaMuerto = true;
        ani.SetBool("walk", false);
        ani.SetTrigger("Die");

        if (fuenteAudio != null && sonidoMuerte != null)
        {
            fuenteAudio.PlayOneShot(sonidoMuerte);
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        StartCoroutine(EsperarYCambiarEscena());
    }

    IEnumerator EsperarYCambiarEscena()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("victoria");
    }
}