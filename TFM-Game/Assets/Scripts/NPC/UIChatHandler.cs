using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIChatHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Button sendButton;

    [SerializeField]
    private Button audioButton;

    [SerializeField]
    private Transform chatContent;

    [SerializeField]
    private GameObject userMessagePrefab;

    [SerializeField]
    private GameObject npcMessagePrefab;

    private string currentNpcId;

    [SerializeField]
    private InputActionReference cancelAction;

    [SerializeField]
    private MissionUI missionUI;

    [SerializeField]
    private Sprite startRecordingSprite;

    [SerializeField]
    private Sprite stopRecordingSprite;

    [SerializeField]
    private ScrollRect chatScrollRect;

    private bool IsRecording = false;
    private Dictionary<string, List<ChatMessage>> npcChats = new();

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
        audioButton.onClick.AddListener(ToggleAudioRecording);
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

        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }

    private void OnNPCResponse(ChatResponse response, bool isAudioResponse = false)
    {
        if (response == null)
        {
            AddMessageToChat("[Connection error]", false);
            return;
        }

        if (isAudioResponse)
        {
            AddMessageToChat(response.user_message, true);
        }
        AddMessageToChat(response.response, false);
        Debug.Log("NPC mood: " + response.mood);
    }

    private void AddMessageToChat(string text, bool isUser)
    {
        npcChats[currentNpcId].Add(new ChatMessage(isUser, text));
        RefreshChatDisplay();
    }

    public void Open(string npcId)
    {
        currentNpcId = npcId;
        panel.SetActive(true);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();

        if (!npcChats.ContainsKey(npcId))
        {
            npcChats[npcId] = new List<ChatMessage>();
        }

        RefreshChatDisplay();
    }

    private void RefreshChatDisplay()
    {
        foreach (Transform child in chatContent)
            Destroy(child.gameObject);

        if (string.IsNullOrEmpty(currentNpcId) || !npcChats.ContainsKey(currentNpcId))
            return;

        foreach (var msg in npcChats[currentNpcId])
        {
            GameObject prefab = msg.isUser ? userMessagePrefab : npcMessagePrefab;
            GameObject msgInstance = Instantiate(prefab, chatContent);
            TMP_Text msgText = msgInstance.GetComponentInChildren<TMP_Text>();
            msgText.text = msg.message;
        }
        StartCoroutine(ScrollToBottom());
    }

    public void Close()
    {
        panel.SetActive(false);
        inputField.DeactivateInputField();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Close();
    }

    private void OnGetCurrentMission(MissionData mission)
    {
        if (MissionManager.Instance.HasActiveMission())
        {
            if (MissionManager.Instance.IsMissionComplete())
            {
                MissionManager.Instance.CompleteMission();
                missionUI.Hide();
                AddMessageToChat("[Misión completada con éxito]", false);
            }
            return;
        }

        if (mission.description != null && MissionManager.Instance.AssignMission(mission))
        {
            missionUI.ShowMission(mission);
        }
    }

    private void OnSubmit(string text)
    {
        OnSendClicked();
    }

    public void ToggleAudioRecording()
    {
        if (IsRecording)
        {
            AudioRecorder.Instance.StopAndSend(
                currentNpcId,
                (response) =>
                {
                    OnNPCResponse(response, true);
                    APIManager.Instance.GetCurrentMission(currentNpcId, OnGetCurrentMission);
                }
            );
            IsRecording = false;
            audioButton.image.sprite = startRecordingSprite;
            return;
        }
        AudioRecorder.Instance.StartRecording();
        IsRecording = true;
        audioButton.image.sprite = stopRecordingSprite;
    }

    public void TryCompleteMission()
    {
        if (!MissionManager.Instance.HasActiveMission())
        {
            return;
        }

        if (!MissionManager.Instance.IsMissionComplete())
        {
            AddMessageToChat("[You did not complete all the objectives]", false);
            return;
        }

        APIManager.Instance.CompleteMissionRequest(
            currentNpcId,
            response =>
            {
                if (response == null)
                {
                    AddMessageToChat("[Error completing the mission]", false);
                    return;
                }

                MissionManager.Instance.CompleteMission();
                missionUI.Hide();
                OnNPCResponse(response);
            }
        );
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
}
