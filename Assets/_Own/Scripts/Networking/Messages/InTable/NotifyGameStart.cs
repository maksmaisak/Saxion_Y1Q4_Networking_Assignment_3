
/// Sent to the players at a table where a game starts.
public class NotifyGameStart : NetworkMessage<NotifyGameStart>
{
    public uint whitePlayerId;
    
    public NotifyGameStart() {}

    public NotifyGameStart(uint whitePlayerId)
    {
        this.whitePlayerId = whitePlayerId;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref whitePlayerId);
    }
}