using System;

public class NewChatMessageClientToServer : NetworkMessage<NewChatMessageClientToServer>
{
    public string message;

    public NewChatMessageClientToServer() {}

    public NewChatMessageClientToServer(string message)
    {
        this.message = message;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref message);
    }
}