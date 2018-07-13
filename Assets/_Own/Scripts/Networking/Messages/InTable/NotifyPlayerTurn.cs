
public class NotifyPlayerTurn : NetworkMessage<NotifyPlayerTurn>
{
    public uint playerId;
    public string playerNickname;
    
    public NotifyPlayerTurn() {}

    public NotifyPlayerTurn(uint playerId, string playerNickname)
    {
        this.playerId = playerId;
        this.playerNickname = playerNickname;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerId);
        s.Serialize(ref playerNickname);
    }
}