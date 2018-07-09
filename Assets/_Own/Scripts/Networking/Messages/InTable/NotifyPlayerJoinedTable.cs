
public class NotifyPlayerJoinedTable : NetworkMessage<NotifyPlayerJoinedTable>
{
    public uint playerId;
    public string nickname;
    
    public NotifyPlayerJoinedTable() {}
    
    public NotifyPlayerJoinedTable(uint playerId, string nickname)
    {
        this.playerId = playerId;
        this.nickname = nickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerId);
        s.Serialize(ref nickname);
    }
}