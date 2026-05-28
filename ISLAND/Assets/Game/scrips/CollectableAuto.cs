using UnityEngine;

public class ColectableAutoZona : MonoBehaviour
{
    [Header("Recoleccion")]
    public int pointValue = 1;
    public float detectionRange = 2f;
    [Header("Zona Secreta")]
    public GameObject[] plataformasAlternativas;
    [Header("Animacion flotante")]
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;
    [Header("Particulas")]
    public ParticleSystem particlesPrefab;
    public Color particleColor = Color.cyan;
    [Header("Checkpoint")]
    public bool esCheckpoint = false;
    public float checkpointOffsetY = 1.5f;

    private Vector3 startPos;
    private bool collected = false;
    private Transform jugador;
    private string uniqueID;

    void Start()
    {
        startPos = transform.position;
        uniqueID = $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_{gameObject.name}_{startPos.x:F2}_{startPos.y:F2}_{startPos.z:F2}";

        if (GameManager.Instance != null && GameManager.Instance.IsCollectibleCollected(uniqueID))
        {
            foreach (GameObject p in plataformasAlternativas)
                if (p != null) p.SetActive(true);
            Destroy(gameObject);
            return;
        }

        foreach (GameObject p in plataformasAlternativas)
            if (p != null) p.SetActive(false);

        GameObject p2 = GameObject.FindGameObjectWithTag("Player");
        if (p2 != null) jugador = p2.transform;
    }

    void Update()
    {
        if (collected) return;
        transform.position = new Vector3(startPos.x, startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight, startPos.z);
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        if (jugador != null && Vector3.Distance(transform.position, jugador.position) <= detectionRange)
            Collect();
    }

    void Collect()
    {
        if (collected) return;
        collected = true;
        if (esCheckpoint)
        {
            Vector3 spawnPos = startPos + Vector3.up * checkpointOffsetY;
            CheckpointManager.Instance?.SetCheckpoint(spawnPos);
            Debug.Log($"[ColectableZona] Checkpoint guardado en: {spawnPos}");
        }
        foreach (GameObject p in plataformasAlternativas)
            if (p != null) p.SetActive(true);
        SpawnParticles();
        if (GameManager.Instance != null)
            GameManager.Instance.RecordCollectible(uniqueID, pointValue);
        UIManager.Instance?.AgregarGema(pointValue);
        Destroy(gameObject, 0.1f);
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
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;
            ps.Play();
            Destroy(psObj, 3f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
