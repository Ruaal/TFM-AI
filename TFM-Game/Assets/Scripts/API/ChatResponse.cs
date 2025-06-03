using System;

[Serializable]
public class ChatResponse
{
    public string user_message;
    public string response;
    public string mood;
    public MissionData missionData;
}
