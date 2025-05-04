using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text interactPrompt;

    [SerializeField]
    private LayerMask NPCLayers;

    [SerializeField]
    private float detectionRadius = 1f;

    [SerializeField]
    private APIManager chatUI;

    private Collider _nearNPC;

    [SerializeField]
    private InputActionReference interactAction;
    private bool _isInteracting = false;

    private void Start()
    {
        string key = GetInteractKeyDisplayName();
        interactPrompt.text = $"Press {key} to interact";
        interactPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isInteracting)
        {
            CheckNPCNearby();
        }
    }

    private void CheckNPCNearby()
    {
        Vector3 origin = gameObject.transform.position;
        Collider[] hits = Physics.OverlapSphere(
            origin,
            detectionRadius,
            NPCLayers,
            QueryTriggerInteraction.Ignore
        );

        float shortestDistance = detectionRadius;
        Collider closest = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(origin, hit.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closest = hit;
            }
        }

        _nearNPC = closest;
        interactPrompt.gameObject.SetActive(_nearNPC != null);
    }

    private string GetInteractKeyDisplayName()
    {
        var binding = interactAction.action.bindings[0];
        return InputControlPath.ToHumanReadableString(
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && _nearNPC != null)
        {
            _isInteracting = true;
            interactPrompt.gameObject.SetActive(false);
            _nearNPC.GetComponent<NPCController>()?.interact();
        }
    }
}
