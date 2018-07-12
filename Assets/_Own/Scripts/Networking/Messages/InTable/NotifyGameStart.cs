
/// Sent to the players at a table where a game starts.
public class NotifyGameStart : NetworkMessage<NotifyGameStart>
{
    public Checkerboard checkerboard;
    public uint whitePlayerId;
    
    public NotifyGameStart() {}

    public NotifyGameStart(Checkerboard checkerboard, uint whitePlayerId)
    {
        this.checkerboard  = checkerboard;
        this.whitePlayerId = whitePlayerId;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref checkerboard);
        s.Serialize(ref whitePlayerId);
    }
}