using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Assertions;

public class Client : Singleton<Client>, IAgent
{
    public FiniteStateMachine<Client> fsm { get; private set; }
    public Connection connectionToServer { get; private set; }
    
    void Start()
    {        
        fsm = new FiniteStateMachine<Client>(this);
        fsm.ChangeState<ClientNotConnectedState>();
    }

    public void SetConnectionToServer(Connection connection)
    {
        Assert.IsNull(connectionToServer, "Connection to server is already assigned.");

        connectionToServer = connection;
    }
}
