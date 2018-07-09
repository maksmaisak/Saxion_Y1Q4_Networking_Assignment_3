
/// Sent to the players at a table where a game starts.
public class NotifyGameStart : NetworkMessage<NotifyGameStart>
{
    public uint startingPlayerId;
    
    public NotifyGameStart() {}

    public NotifyGameStart(uint startingPlayerId)
    {
        this.startingPlayerId = startingPlayerId;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref startingPlayerId);
    }
}