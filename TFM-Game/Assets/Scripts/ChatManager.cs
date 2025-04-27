using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;
    public string apiUrl = "http://localhost:5000/npc/Aria/message";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator SendMessageToNPC(string userMessage, System.Action<ChatResponse> callback)
    {
        ChatRequest requestData = new ChatRequest { message = userMessage };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error contacting server: " + request.error);
            callback(null);
        }
        else
        {
            ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
            callback(responseData);
        }
    }
}
