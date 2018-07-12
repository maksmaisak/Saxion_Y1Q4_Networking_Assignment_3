
/// Sent to all players after they join a table.
public class NotifyTableState : NetworkMessage<NotifyTableState>
{
    public Checkerboard checkerboard;
    public PlayerInfo? otherPlayerInfo;
        
    public NotifyTableState() {}

    public NotifyTableState(Checkerboard checkerboard, PlayerInfo? otherPlayerInfo = null)
    {
        this.checkerboard = checkerboard;
        this.otherPlayerInfo  = otherPlayerInfo;
    }
        
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref checkerboard);

        // TODO Extract this into an extension mehtod for serializing any nullable.
        bool hasOtherPlayer = otherPlayerInfo.HasValue;
        s.Serialize(ref hasOtherPlayer);
        if (hasOtherPlayer)
        {
            PlayerInfo value = s.isWriting ? otherPlayerInfo.Value : new PlayerInfo();
            s.Serialize(ref value);
            otherPlayerInfo = value;
        }
    }
}
