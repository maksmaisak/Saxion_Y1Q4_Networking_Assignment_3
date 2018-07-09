
public class NotifyPlayerLeftTable : NetworkMessage<NotifyPlayerLeftTable>
{
    public string nickname;
    
    public NotifyPlayerLeftTable() {}

    public NotifyPlayerLeftTable(string nickname)
    {
        this.nickname = nickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nickname);
    }
}