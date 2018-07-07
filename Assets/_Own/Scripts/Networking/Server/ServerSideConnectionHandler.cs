using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Connection))]
public class ServerSideConnectionHandler : MonoBehaviour, IAgent, IEventReceiver<INetworkMessage>
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
        if (connection.state == Connection.State.Disconnected)
        {
            Destroy(gameObject);
        }
    }
    
    public void On(INetworkMessage eventData)
    {
        if (eventData.originConnection != connection) return;

        bool isValidMessage = new System.Type[]
        {
            typeof(ClientHandshake)
        }.Contains(eventData.GetType());
        // TODO
    }
}