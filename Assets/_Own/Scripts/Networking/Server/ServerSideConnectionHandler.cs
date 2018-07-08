using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Connection))]
public class ServerSideConnectionHandler : MonoBehaviour, IAgent, 
    IEventReceiver<DisconnectMessage>
{
    public FiniteStateMachine<ServerSideConnectionHandler> fsm { get; private set; }
    public Connection connection { get; private set; }
    public string clientNickname { get; set; }
    
    void Start()
    {
        fsm = new FiniteStateMachine<ServerSideConnectionHandler>(this);
        
        connection = GetComponent<Connection>();
        Assert.IsNotNull(connection);
        
        fsm.ChangeState<ServerConnectionStateAwaitingClientHandshake>();
    }

    void FixedUpdate()
    {
        // TODO ? Have a state to do the disconnecting.
        if (connection.state == Connection.State.Closed) HandleDisconnect();
    }
    
    public void On(DisconnectMessage eventData)
    {
        connection.Close();
    }

    private void HandleDisconnect()
    {
        if (clientNickname != null)
        {
            Server.Instance.state.RemoveNickname(clientNickname);
        }
        
        Destroy(gameObject);
    }
}