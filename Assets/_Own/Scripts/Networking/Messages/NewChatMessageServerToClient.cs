using System;
using System.Globalization;

public class NewChatMessageServerToClient : NetworkMessage<NewChatMessageServerToClient>
{
    public enum Kind
    {
        Message,
        Whisper,
        ServerMessage
    }

    public Kind kind = Kind.Message;
    public string message;
    
    public static NewChatMessageServerToClient MakeWithTimestamp(string message, Kind kind = Kind.Message)
    {
        return new NewChatMessageServerToClient($"{GetTimestampNow()} {message}", kind);
    }

    public NewChatMessageServerToClient() {}

    public NewChatMessageServerToClient(string message, Kind kind = Kind.Message)
    {
        this.kind = kind;
        this.message = message;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        int value = (int)kind;
        s.Serialize(ref value);
        kind = (Kind)value;
        
        s.Serialize(ref message);
    }
    
    private static string GetTimestampNow()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}