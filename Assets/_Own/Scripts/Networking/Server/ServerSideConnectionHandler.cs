using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Connection))]
public class ServerSideConnectionHandler : MonoBehaviour, IAgent, IEventReceiver<DisconnectMessage>
{
    public FiniteStateMachine<ServerSideConnectionHandler> fsm { get; private set; }
    public Connection connection { get; private set; }
    public string clientNickname { get; set; }
    
    void Start()
    {
        fsm = new FiniteStateMachine<ServerSideConnectionHandler>(this);
        
        connection = GetComponent<Connection>();
        Assert.IsNotNull(connection);
        
        fsm.ChangeState<AwaitingClientHandshake>();
    }

    void FixedUpdate()
    {
        if (connection.state != Connection.State.Closed) return;
        
        if (clientNickname != null)
        {
            Server.Instance.state.RemoveNickname(clientNickname);
        }

        Destroy(gameObject);
    }
    
    public void On(DisconnectMessage eventData)
    {
        connection.Close();
    }
}