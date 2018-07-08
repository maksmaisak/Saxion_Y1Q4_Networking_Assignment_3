
public class UserJoinedMessage : NetworkMessage<UserJoinedMessage>
{
    public string nickname;
    
    public UserJoinedMessage() {}
    
    public UserJoinedMessage(string nickname)
    {
        this.nickname = nickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nickname);
    }
}