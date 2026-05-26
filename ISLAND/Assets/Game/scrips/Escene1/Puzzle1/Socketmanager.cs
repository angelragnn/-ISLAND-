using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SocketReceiver : MonoBehaviour
{
    public string acceptedID;
    public ParticleSystem auraParticles;
    public AudioClip soundReady;
    public AudioClip soundCorrect;
    public AudioClip soundWrong;

    private bool activated = false;
    private bool playerInside = false;
    private bool readySoundPlayed = false;
    private PickUpSystem pickUp;
    private Scene1Controller scene1Controller; // Referencia automática al controlador de escena
    private AudioSource audioSource;

    void Start()
    {
        pickUp = FindFirstObjectByType<PickUpSystem>();
        scene1Controller = FindFirstObjectByType<Scene1Controller>(); // Buscar el controlador en la escena automáticamente
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        SetAuraColor(new Color(0.3f, 0.3f, 1f));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (pickUp != null)
                pickUp.SetInsideSocket(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            readySoundPlayed = false;
            if (pickUp != null)
                pickUp.SetInsideSocket(false);
            if (!activated)
                SetAuraColor(new Color(0.3f, 0.3f, 1f));
        }
    }

    void Update()
    {
        if (activated) return;
        if (!playerInside) return;
        if (pickUp == null) return;

        if (pickUp.heldObject == null)
        {
            readySoundPlayed = false;
            SetAuraColor(new Color(0.3f, 0.3f, 1f));
            return;
        }

        GrabbableObject held = pickUp.heldObject.GetComponent<GrabbableObject>();
        if (held == null) return;

        if (held.objectID == acceptedID)
        {
            SetAuraColor(new Color(0f, 1f, 0f));

            if (!readySoundPlayed)
            {
                if (soundReady != null)
                    audioSource.PlayOneShot(soundReady);
                readySoundPlayed = true;
            }

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                StartCoroutine(PlaceObject(held));
        }
        else
        {
            readySoundPlayed = false;
            SetAuraColor(new Color(1f, 0f, 0f));

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                StartCoroutine(RejectObject(held));
        }
    }

    IEnumerator RejectObject(GrabbableObject obj)
    {
        if (soundWrong != null)
            audioSource.PlayOneShot(soundWrong);

        pickUp.ForceRelease();

        obj.rb.isKinematic = false;
        obj.rb.useGravity = true;
        obj.col.enabled = true;
        obj.transform.SetParent(null);

        Vector3 dir = (pickUp.transform.position - obj.transform.position).normalized;
        obj.rb.AddForce(dir * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        SetAuraColor(new Color(0.3f, 0.3f, 1f));
    }

    IEnumerator PlaceObject(GrabbableObject obj)
    {
        activated = true;
        pickUp.ForceRelease();
        pickUp.SetInsideSocket(false);

        if (soundCorrect != null)
            audioSource.PlayOneShot(soundCorrect);

        obj.rb.isKinematic = true;
        obj.col.enabled = false;
        obj.transform.SetParent(null);

        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = transform.position + Vector3.up * 0.5f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            obj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        obj.transform.position = targetPos;
        obj.transform.SetParent(transform);

        SetAuraColor(new Color(1f, 0.8f, 0f));

        // NOTIFICAR AL CONTROLADOR DE ESCENA
        if (scene1Controller != null)
        {
            scene1Controller.OnSocketActivated();
        }
    }

    void SetAuraColor(Color c)
    {
        if (auraParticles == null) return;
        var main = auraParticles.main;
        main.startColor = c;
    }
}