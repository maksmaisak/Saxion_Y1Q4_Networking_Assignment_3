using System;
using System.Collections.Generic;

public class ConnectionMessageVerifier
{
    private readonly HashSet<Type> whitelist = new HashSet<Type>();
    //private readonly HashSet<Type> blacklist = new HashSet<Type>();
    
    public bool Verify(INetworkMessage message)
    {
        return whitelist.Contains(message.GetType());
    }
}