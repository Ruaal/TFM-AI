using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIChatHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject panel;
    public Button sendButton;
    public Transform chatContent;
    public GameObject userMessagePrefab;
    public GameObject npcMessagePrefab;

    private string currentNpcId;

    [SerializeField]
    private InputActionReference cancelAction;

    [SerializeField]
    private MonoBehaviour movementController;

    [SerializeField]
    private MissionUI missionUI;

    private void OnEnable()
    {
        cancelAction.action.Enable();

        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        inputField.onSubmit.AddListener(OnSubmit);
        panel.SetActive(false);
    }

    private void OnSendClicked()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        string userText = inputField.text.Trim();
        AddMessageToChat(userText, isUser: true);
        APIManager.Instance.SendMessageToNPC(
            userText,
            currentNpcId,
            (response) =>
            {
                OnNPCResponse(response);
                APIManager.Instance.GetCurrentMission(currentNpcId, OnGetCurrentMission);
            }
        );

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void OnNPCResponse(ChatResponse response)
    {
        if (response == null)
        {
            AddMessageToChat("[Connection error]", isUser: false);
            return;
        }

        AddMessageToChat(response.response, isUser: false);
        Debug.Log("NPC mood: " + response.mood);
    }

    private void AddMessageToChat(string text, bool isUser)
    {
        GameObject prefab = isUser ? userMessagePrefab : npcMessagePrefab;
        GameObject msgInstance = Instantiate(prefab, chatContent);
        TMP_Text msgText = msgInstance.GetComponentInChildren<TMP_Text>();
        msgText.text = text;
    }

    public void Open(string npcId)
    {
        currentNpcId = npcId;
        panel.SetActive(true);
        inputField.ActivateInputField();

        if (movementController != null)
            movementController.enabled = false;
    }

    public void Close()
    {
        panel.SetActive(false);
        inputField.DeactivateInputField();

        if (movementController != null)
            movementController.enabled = true;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Close();
    }

    private void OnGetCurrentMission(MissionData mission)
    {
        if (mission != null)
        {
            missionUI.ShowMission(mission);
        }
    }

    private void OnSubmit(string text)
    {
        OnSendClicked();
    }
}
