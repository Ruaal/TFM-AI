using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FlaskLauncher : MonoBehaviour
{
    private Process flaskProcess;

    [SerializeField]
    private List<string> npcIdsToReset = new() { "Aria", "Bruce", "Kai" };

    private void Start()
    {
        StartFlask();
        DontDestroyOnLoad(gameObject);
        StartCoroutine(ResetAllNPCStates(2f));
    }

    private void OnApplicationQuit()
    {
        KillFlask();
    }

    private void OnDestroy()
    {
        KillFlask();
    }

    private void StartFlask()
    {
#if UNITY_EDITOR
        string scriptPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "..", "TFM-API", "start_api.bat")
        );
#else
        string scriptPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "TFM-API", "start_api.bat")
        );
#endif

        flaskProcess = new Process();
        flaskProcess.StartInfo.FileName = scriptPath;
        flaskProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptPath);
        flaskProcess.StartInfo.CreateNoWindow = true;
        flaskProcess.StartInfo.UseShellExecute = false;
        flaskProcess.StartInfo.RedirectStandardOutput = true;
        flaskProcess.StartInfo.RedirectStandardError = true;
        flaskProcess.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
                UnityEngine.Debug.Log(e.Data);
        };
        flaskProcess.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null)
                UnityEngine.Debug.LogError(e.Data);
        };
        flaskProcess.Start();
        flaskProcess.BeginOutputReadLine();
        flaskProcess.BeginErrorReadLine();

        UnityEngine.Debug.Log("Flask API started with setup script.");
    }

    private IEnumerator ResetAllNPCStates(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (string npcId in npcIdsToReset)
        {
            string url = $"http://127.0.0.1:5000/npc/{npcId}/state";
            UnityWebRequest request = UnityWebRequest.Delete(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                UnityEngine.Debug.LogError($"[NPC State Reset] {npcId} -> {request.error}");
            else
                UnityEngine.Debug.Log($"[NPC State Reset] {npcId} state cleared.");
        }
    }

    private void KillFlask()
    {
        if (flaskProcess != null && !flaskProcess.HasExited)
        {
            flaskProcess.Kill();
            flaskProcess.Dispose();
            flaskProcess = null;
            UnityEngine.Debug.Log("[FlaskLauncher] Flask cerrado correctamente.");
        }
    }
}
