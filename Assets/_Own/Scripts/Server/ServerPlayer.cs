using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Connection))]
public class ServerPlayer : MyBehaviour, IAgent, 
    IEventReceiver<DisconnectMessage>
{
    public FiniteStateMachine<ServerPlayer> fsm { get; private set; }
    public Connection connection { get; private set; }

    public uint   playerId { get; set; }
    public string nickname { get; set; }
    
    void Start()
    {
        fsm = new FiniteStateMachine<ServerPlayer>(this);
        
        connection = GetComponent<Connection>();
        Assert.IsNotNull(connection);
        
        fsm.ChangeState<ServerPlayerStateAwaitingServerJoinRequest>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        var server = Server.Instance;
        if (server) server.joinedPlayers.Remove(this);
    }

    void FixedUpdate()
    {
        // TODO ? Have a ServerPlayerAwaitingReconnect state for waiting for a client to reconnect.
        if (connection.state == Connection.State.Closed) HandleDisconnect();
    }
    
    public void On(DisconnectMessage request)
    {
        connection.Close();
    }

    private void HandleDisconnect()
    {        
        Destroy(gameObject);
    }
}