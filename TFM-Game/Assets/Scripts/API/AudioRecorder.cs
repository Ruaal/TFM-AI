using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public static AudioRecorder Instance;

    [SerializeField]
    private int maxDurationSeconds = 10;

    [SerializeField]
    private int sampleRate = 16000;

    private AudioClip recordedClip;
    private string microphone;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartRecording()
    {
        microphone = Microphone.devices[0];
        recordedClip = Microphone.Start(microphone, false, maxDurationSeconds, sampleRate);
        Debug.Log("Recording started...");
    }

    public void StopAndSend(string npcId, System.Action<ChatResponse> callback)
    {
        if (!Microphone.IsRecording(microphone))
            return;

        Microphone.End(microphone);

        APIManager.Instance.SendAudioToNPC(npcId, recordedClip, callback);
    }
}
