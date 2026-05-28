using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    [Header("Configuración de Respawn")]
    [SerializeField] private Transform puntoDeRespawn; // Arrastra aquí tu GameObject vacío

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1. Gestionar la vida
        LifeManager.Instance?.PerderVida();

        // 2. Mover al jugador al punto de respawn
        if (puntoDeRespawn != null)
        {
            // Desactivamos el CharacterController/Rigidbody temporalmente 
            // para evitar conflictos con la física al teletransportar
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