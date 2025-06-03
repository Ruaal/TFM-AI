using System;
using System.Collections;
using System.IO;
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

    public void SendAudioToNPC(string npcId, AudioClip clip, System.Action<ChatResponse> callback)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        byte[] audioData = ConvertToWav(samples, clip.channels, clip.frequency);
        StartCoroutine(SendAudioCoroutine(npcId, audioData, callback));
    }

    private IEnumerator SendAudioCoroutine(
        string npcId,
        byte[] audioData,
        System.Action<ChatResponse> callback
    )
    {
        string url = $"{baseApiUrl}/npc/{npcId}/audio_message";
        WWWForm form = new WWWForm();
        form.AddBinaryData("audioClip", audioData, "audio.wav", "audio/wav");
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(
                www.downloadHandler.text
            );
            callback(responseData);
        }
    }

    byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();
        int samplesCount = samples.Length;
        int byteRate = sampleRate * channels * 2;
        int subChunk2 = samplesCount * 2;
        int chunkSize = 36 + subChunk2;

        // WAV header
        stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(chunkSize), 0, 4);
        stream.Write(Encoding.ASCII.GetBytes("WAVEfmt "), 0, 8);
        stream.Write(System.BitConverter.GetBytes(16), 0, 4);
        stream.Write(System.BitConverter.GetBytes((short)1), 0, 2);
        stream.Write(System.BitConverter.GetBytes((short)channels), 0, 2);
        stream.Write(System.BitConverter.GetBytes(sampleRate), 0, 4);
        stream.Write(System.BitConverter.GetBytes(byteRate), 0, 4);
        stream.Write(System.BitConverter.GetBytes((short)(channels * 2)), 0, 2);
        stream.Write(System.BitConverter.GetBytes((short)16), 0, 2);
        stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(subChunk2), 0, 4);

        // PCM samples
        for (int i = 0; i < samples.Length; i++)
        {
            short intData = (short)Mathf.Clamp(samples[i] * 32767, -32768, 32767);
            stream.Write(System.BitConverter.GetBytes(intData), 0, 2);
        }
        return stream.ToArray();
    }

    public void CompleteMissionRequest(string npcId, Action<ChatResponse> callback)
    {
        StartCoroutine(CompleteMissionCoroutine(npcId, callback));
    }

    private IEnumerator CompleteMissionCoroutine(string npcId, Action<ChatResponse> callback)
    {
        string url = $"http://127.0.0.1:5000/npc/{npcId}/complete_mission";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[Mission Completion Error] {request.error}");
            callback?.Invoke(null);
        }
        else
        {
            ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(
                request.downloadHandler.text
            );
            callback?.Invoke(responseData);
        }
    }
}
