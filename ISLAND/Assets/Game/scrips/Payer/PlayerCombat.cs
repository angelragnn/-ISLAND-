using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;

    [Header("Ajustes de Ataque")]
    public float rangoAtaque = 1.5f;
    public float distanciaAdelante = 1.2f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Atacar");
        }
    }

    public void RealizarGolpe()
    {
        Vector3 puntoAtaque = transform.position + transform.forward * distanciaAdelante;
        Collider[] objetosGolpeados = Physics.OverlapSphere(puntoAtaque, rangoAtaque);

        foreach (Collider col in objetosGolpeados)
        {
            Enemigo enemigo = col.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(1);
            }

            EsqueletoNegro enemigoNegro = col.GetComponent<EsqueletoNegro>();
            if (enemigoNegro != null)
            {
                enemigoNegro.RecibirDano(1);
            }

            JefeFinal jefe = col.GetComponent<JefeFinal>();
            if (jefe != null)
            {
                jefe.RecibirDano(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 puntoAtaque = transform.position + transform.forward * distanciaAdelante;
        Gizmos.DrawWireSphere(puntoAtaque, rangoAtaque);
    }
}