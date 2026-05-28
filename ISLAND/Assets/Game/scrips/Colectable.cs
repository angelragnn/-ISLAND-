using UnityEngine;

// ============================================================
//  COLLECTIBLE — Poner este script en cada objeto recolectable
//  El objeto DEBE tener el tag "Recolectable"
//  y un Collider con isTrigger = true
// ============================================================
public class Collectible : MonoBehaviour
{
    [Header("Configuracion")]
    public int pointValue = 1;
    public Color particleColor = Color.cyan;

    [Header("Particulas")]
    public ParticleSystem particlesPrefab;   // Arrastra un Particle System desde el Inspector

    [Header("Animacion flotante")]
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;

    private Vector3 startPos;
    private bool collected = false;

    void Start()
    {
        // Asegurarse de que tenga el tag correcto
        if (!gameObject.CompareTag("Recolectable"))
            gameObject.tag = "Recolectable";

        startPos = transform.position;
    }

    void Update()
    {
        if (collected) return;

        // Animacion flotante
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    /// <summary>Llamado desde GameManager via Raycast O por trigger directo</summary>
    public void Collect()
    {
        if (collected) return;
        collected = true;

        // Lanzar particulas del color asignado
        SpawnParticles();

        // Notificar al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectibleCollected(pointValue);

        // Destruir objeto
        Destroy(gameObject, 0.1f);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }

    void SpawnParticles()
    {
        if (particlesPrefab != null)
        {
            ParticleSystem ps = Instantiate(particlesPrefab, transform.position, Quaternion.identity);

           
            var main = ps.main;
            main.startColor = particleColor;

            ps.Play();
            Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax);
        }
        else
        {
           
            GameObject psObj = new GameObject("Particles_" + gameObject.name);
            psObj.transform.position = transform.position;

            ParticleSystem ps = psObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = particleColor;
            main.startSize = 0.3f;
            main.startSpeed = 3f;
            main.startLifetime = 1.5f;
            main.duration = 0.5f;
            main.loop = false;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 20)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            ps.Play();
            Destroy(psObj, 3f);
        }
    }
}