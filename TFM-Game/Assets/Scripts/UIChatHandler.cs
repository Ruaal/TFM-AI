using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIChatHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public Transform chatContent;
    public GameObject userMessagePrefab;
    public GameObject npcMessagePrefab;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnSendClicked()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        string userText = inputField.text.Trim();
        AddMessageToChat(userText, isUser: true);
        StartCoroutine(ChatManager.Instance.SendMessageToNPC(userText, OnNPCResponse));

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
}
