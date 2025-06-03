using System;

[Serializable]
public class ChatMessage
{
    public bool isUser;
    public string message;

    public ChatMessage(bool isUser, string message)
    {
        this.isUser = isUser;
        this.message = message;
    }
}
