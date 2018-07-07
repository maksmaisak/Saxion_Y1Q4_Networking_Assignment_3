using System;
using UnityEngine.Assertions;

/// A message to be sent over the network. Must have a public argument-less constructor (new()).
public interface INetworkMessage : IBroadcastEvent, IUnifiedSerializable
{
    Connection originConnection { get; }
    void InitializeOnReceived(Connection originConnection);
}

public abstract class NetworkMessage<T> : BroadcastEvent<T>, INetworkMessage 
    where T : NetworkMessage<T>
{
    public Connection originConnection { get; private set; }
    private bool isInitializedOnReceived;
    
    public void InitializeOnReceived(Connection originConnection)
    {
        Assert.IsFalse(isInitializedOnReceived, this + " has already been initialized.");
        this.originConnection = originConnection;
        isInitializedOnReceived = true;
    }
    
    public abstract void Serialize(IUnifiedSerializer s);
}