using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Connection))]
public class ServerSideConnectionHandler : MonoBehaviour, IAgent
{
    public FiniteStateMachine<ServerSideConnectionHandler> fsm { get; private set; }
    public Connection connection { get; private set; }
    
    void Start()
    {
        fsm = new FiniteStateMachine<ServerSideConnectionHandler>(this);
        
        connection = GetComponent<Connection>();
        Assert.IsNotNull(connection);
        
        fsm.ChangeState<AwaitingClientHandshake>();
    }
}