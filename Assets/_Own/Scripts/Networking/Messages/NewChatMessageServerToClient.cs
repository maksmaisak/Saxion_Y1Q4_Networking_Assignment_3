using System;
using System.Globalization;

public class NewChatMessageServerToClient : NetworkMessage<NewChatMessageServerToClient>
{
    public string message;

    public NewChatMessageServerToClient() {}

    public NewChatMessageServerToClient(string message)
    {
        this.message = message;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref message);
    }
}