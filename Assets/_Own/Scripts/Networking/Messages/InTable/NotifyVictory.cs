
public class NotifyVictory : NetworkMessage<NotifyVictory>
{
    public uint playerId;
     
    public NotifyVictory() {}

    public NotifyVictory(uint playerId)
    {
        this.playerId = playerId;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref playerId);
    }
}