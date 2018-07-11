
public class NotifyPlayerTurn : NetworkMessage<NotifyPlayerTurn>
{
    public uint playerId;
    
    public NotifyPlayerTurn() {}

    public NotifyPlayerTurn(uint playerId)
    {
        this.playerId = playerId;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerId);
    }
}