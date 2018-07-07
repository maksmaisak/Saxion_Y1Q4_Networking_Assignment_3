using System;
using System.Globalization;

public class NewChatMessageServerToClient : NetworkMessage<NewChatMessageServerToClient>
{
    public DateTime timestamp;
    public string nickname;
    public string message;

    public NewChatMessageServerToClient() {}

    public string GetChatLine()
    {
        return $"{timestamp.TimeOfDay} {nickname}: {message}";
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        SerializeTimestamp(s);

        s.Serialize(ref nickname);
        s.Serialize(ref message);
    }

    private void SerializeTimestamp(IUnifiedSerializer s)
    {
        string value = null;
        if (s.isWriting)
        {
            value = timestamp.ToShortTimeString();
            s.Serialize(ref value);
        }
        else
        {
            s.Serialize(ref value);
            timestamp = DateTime.Parse(value);
        }
    }
}