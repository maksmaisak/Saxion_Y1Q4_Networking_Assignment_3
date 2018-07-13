
public class NotifyPlayerJoinedTable : NetworkMessage<NotifyPlayerJoinedTable>
{
    public PlayerInfo playerInfo;
    
    public NotifyPlayerJoinedTable() {}
    
    public NotifyPlayerJoinedTable(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerInfo);
    }

    public override string ToString() => $"{base.ToString()}: {playerInfo.id}, {playerInfo.nickname}";
}