
/// Sent to all newly joined clients
/// If accepted, contains the assigned player id and TODO the table list
public class JoinServerResponse : NetworkMessage<JoinServerResponse>
{
    public bool isReject;
    
    public string rejectionMessage;
    public uint playerId;
    
    public static JoinServerResponse MakeAccept(uint playerId)
    {
        return new JoinServerResponse
        {
            isReject = false,
            playerId = playerId
        };
    }

    public static JoinServerResponse MakeReject(string rejectionMessage)
    {
        return new JoinServerResponse
        {
            isReject = true,
            rejectionMessage = rejectionMessage
        };
    }
    
    public JoinServerResponse() {}
        
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref isReject);

        if (isReject)
        {
            s.Serialize(ref rejectionMessage);
        }
        else
        {
            s.Serialize(ref playerId);
        }
    }
}