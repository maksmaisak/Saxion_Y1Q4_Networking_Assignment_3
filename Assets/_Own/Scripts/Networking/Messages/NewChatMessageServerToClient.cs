
public class NewChatMessageServerToClient : NetworkMessage<NewChatMessageServerToClient>
{
    public string nickname;
    public string message;

    public NewChatMessageServerToClient() {}

    public NewChatMessageServerToClient(string nickname, string message)
    {
        this.nickname = nickname;
        this.message  = message;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nickname);
        s.Serialize(ref message);
    }
}