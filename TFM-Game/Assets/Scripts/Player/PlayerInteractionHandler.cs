using StarterAssets;
using TMPro;
using UnityEditor.Rendering;
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

    private Collider _nearNPC;

    [SerializeField]
    private InputActionReference interactAction;

    [SerializeField]
    private InputActionReference openInventoryAction;

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private TMP_InputField chatInputField;

    private bool _isInteracting = false;

    [SerializeField]
    private InputActionReference moveAction;

    [SerializeField]
    private InputActionReference lookAction;

    [SerializeField]
    private InputActionReference jumpAction;

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
            moveAction.action.Disable();
            lookAction.action.Disable();
            jumpAction.action.Disable();
            interactAction.action.Disable();
            openInventoryAction.action.Disable();
            interactPrompt.gameObject.SetActive(false);
            _nearNPC.GetComponent<NPCController>()?.interact();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnCancel(InputValue value)
    {
        if (value.isPressed || _isInteracting)
        {
            _isInteracting = false;
            moveAction.action.Enable();
            lookAction.action.Enable();
            jumpAction.action.Enable();
            interactAction.action.Enable();
            openInventoryAction.action.Enable();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnOpenInventory(InputValue value)
    {
        if (value.isPressed)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                inventoryPanel.GetComponent<InventoryUI>().RefreshUI();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void OnCompleteMission(InputValue value)
    {
        if (value.isPressed && _isInteracting && !chatInputField.isFocused)
        {
            _nearNPC.GetComponent<NPCController>()?.completeMission();
        }
    }
}
