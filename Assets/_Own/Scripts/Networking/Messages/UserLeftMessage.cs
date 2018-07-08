
public class UserLeftMessage : NetworkMessage<UserLeftMessage>
{
    public string nickname;
    
    public UserLeftMessage() {}

    public UserLeftMessage(string nickname)
    {
        this.nickname = nickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nickname);
    }
}