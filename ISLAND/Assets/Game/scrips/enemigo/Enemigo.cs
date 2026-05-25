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

    void Start()
    {
        ani = GetComponent<Animator>();
        ani.SetBool("walk", false);
        ani.SetBool("run", false);
        ani.SetBool("attack", false);
        atacando = false;
    }

    void Update()
    {
        if (target == null) return;
        Comportamiento_Enemigo();
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
}