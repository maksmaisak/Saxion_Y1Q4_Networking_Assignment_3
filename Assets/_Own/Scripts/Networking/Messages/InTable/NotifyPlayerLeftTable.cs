
public class NotifyPlayerLeftTable : NetworkMessage<NotifyPlayerLeftTable>
{
    public uint playerId;
    public string nickname;
    
    public NotifyPlayerLeftTable() {}

    public NotifyPlayerLeftTable(uint playerId, string nickname)
    {
        this.playerId = playerId;
        this.nickname = nickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerId);
        s.Serialize(ref nickname);
    }
    
    public override string ToString() => $"NotifyPlayerLeftTable: {playerId}, {nickname}";
}