
public class DisconnectMessage : NetworkMessage<DisconnectMessage>
{
    public override void Serialize(IUnifiedSerializer s)
    {
        string dummy = "dummy payload";
        s.Serialize(ref dummy);
    }
}