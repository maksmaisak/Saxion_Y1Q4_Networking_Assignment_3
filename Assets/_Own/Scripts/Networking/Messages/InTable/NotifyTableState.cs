
/// Sent to all players after they join a table.
public class NotifyTableState : NetworkMessage<NotifyTableState>
{
    public Checkerboard checkerboard;
    public string playerANickname;
    public string playerBNickname;
    
    public NotifyTableState() {}

    public NotifyTableState(Checkerboard checkerboard)
    {
        this.checkerboard = new Checkerboard(checkerboard);
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref checkerboard);
    }
}
