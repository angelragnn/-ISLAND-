using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerCollector : MonoBehaviour
{
    [Header("Detección")]
    public float detectionRange = 3f;

    [Header("UI")]
    public TextMeshProUGUI interactionText;





    private ElementController currentGem;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        DetectGem();
        HandleInput();
    }

    void DetectGem()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Collectable");
        float closestDistance = detectionRange;
        ElementController closestGem = null;

        foreach (GameObject gemObj in gems)
        {
            float distance = Vector3.Distance(transform.position, gemObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGem = gemObj.GetComponent<ElementController>();
            }
        }

        currentGem = closestGem;
        ShowInteractionPrompt(currentGem != null);
    }

    void HandleInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentGem != null)
        {


            currentGem.Interact();
            currentGem = null;
            ShowInteractionPrompt(false);
        }
    }

    void ShowInteractionPrompt(bool show)
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(show);
    }
}