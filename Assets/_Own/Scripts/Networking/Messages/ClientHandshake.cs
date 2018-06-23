using System;
using UnityEngine.Assertions;

public class ClientHandshake : NetworkMessage<ClientHandshake>
{
    public class InvalidClientHandshakeException : Exception
    {
        public InvalidClientHandshakeException() : base() {}
        public InvalidClientHandshakeException(string message) : base(message) {}
    }
    
    public const uint ProtocolIdentifier = 0xABCDDCBA;
    public uint protocolIdentifier = ProtocolIdentifier;
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref protocolIdentifier);
    }

    public void ThrowIfInvalidProtocolIdentifier()
    {
        if (protocolIdentifier != ProtocolIdentifier)
        {
            throw new InvalidClientHandshakeException(
                $"Invalid protocol identifier. Expected: {ProtocolIdentifier}, got {protocolIdentifier}"
            );
        }
    }
}