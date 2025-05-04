using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;
    private string baseApiUrl = "http://localhost:5000";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SendMessageToNPC(string message, string npcId, System.Action<ChatResponse> callback)
    {
        StartCoroutine(SendMessageCoroutine(message, npcId, callback));
    }

    private IEnumerator SendMessageCoroutine(
        string userMessage,
        string npcId,
        System.Action<ChatResponse> callback
    )
    {
        string apiUrl = $"{baseApiUrl}/npc/{npcId}/message";

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

        ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(
            request.downloadHandler.text
        );
        callback(responseData);
    }

    public void GetCurrentMission(string npcId, System.Action<MissionData> callback)
    {
        StartCoroutine(GetMissionCoroutine(npcId, callback));
    }

    private IEnumerator GetMissionCoroutine(string npcId, System.Action<MissionData> callback)
    {
        string url = $"{baseApiUrl}/npc/{npcId}/mission";
        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[Mission Error] {request.error}");
            callback?.Invoke(null);
            yield break;
        }

        var wrapper = JsonUtility.FromJson<MissionWrapper>(request.downloadHandler.text);
        callback?.Invoke(wrapper.mission);
    }
}
