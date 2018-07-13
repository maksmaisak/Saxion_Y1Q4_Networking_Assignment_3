using System;
using UnityEngine.Assertions;

public class JoinServerRequest : NetworkMessage<JoinServerRequest>
{    
    public const uint CorrectProtocolIdentifier = 0xABCDDCBA;
    public uint protocolIdentifier = CorrectProtocolIdentifier;

    public string nickname;
    
    public JoinServerRequest() {}

    public JoinServerRequest(string nickname)
    {
        this.nickname = nickname;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref protocolIdentifier);
        s.Serialize(ref nickname);
    }

    public bool GetIsValid()
    {
        return protocolIdentifier == CorrectProtocolIdentifier;
    }
}