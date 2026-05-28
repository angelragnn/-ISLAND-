using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    [Header("Configuración de Respawn")]
    [SerializeField] private Transform puntoDeRespawn;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        LifeManager.Instance?.PerderVida();


        if (puntoDeRespawn != null)
        {


            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            other.transform.position = puntoDeRespawn.position;
            other.transform.rotation = puntoDeRespawn.rotation;

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogWarning("No has asignado un punto de respawn en " + gameObject.name);
        }
    }
}