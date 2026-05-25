using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    public GameObject enemigoPrefab;
    public GameObject jugadorTarget;

    [Header("Límites y Tiempos")]
    public int maxEnemigos = 6;
    public float tiempoEntreSpawn = 5f;
    private float cronometro;

    [Header("Zona de Generación")]
    public float radioSpawn = 10f;

    private List<GameObject> enemigosVivos = new List<GameObject>();

    void Update()
    {
        enemigosVivos.RemoveAll(item => item == null);

        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreSpawn && enemigosVivos.Count < maxEnemigos)
        {
            SpawnearEnemigo();
            cronometro = 0;
        }
    }

    void SpawnearEnemigo()
    {
        Vector2 puntoAleatorio = Random.insideUnitCircle * radioSpawn;
        Vector3 posicionSpawn = new Vector3(
            transform.position.x + puntoAleatorio.x,
            transform.position.y,
            transform.position.z + puntoAleatorio.y
        );

        GameObject nuevoEnemigo = Instantiate(enemigoPrefab, posicionSpawn, Quaternion.identity);

        Enemigo scriptEnemigo = nuevoEnemigo.GetComponent<Enemigo>();
        if (scriptEnemigo != null)
        {
            scriptEnemigo.target = jugadorTarget;
        }

        enemigosVivos.Add(nuevoEnemigo);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioSpawn);
    }
}