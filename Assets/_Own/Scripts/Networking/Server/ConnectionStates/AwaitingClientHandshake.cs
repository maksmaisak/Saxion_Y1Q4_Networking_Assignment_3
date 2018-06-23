using System.Collections;
using UnityEngine;

public class AwaitingClientHandshake : 
    FSMState<ServerSideConnectionHandler>, 
    IEventReceiver<ClientHandshake>
{
    [SerializeField] float handshakeTimeout = 1f;
    
    private float timeLeftForHandshake;
    
    public override void Enter()
    {
        base.Enter();
        timeLeftForHandshake = handshakeTimeout;
    }

    void Update()
    {
        timeLeftForHandshake -= Time.deltaTime;
        if (timeLeftForHandshake > 0f) return;

        Debug.Log("Client handshake timeout. Kicking client.");
        KickClient();
    }

    public void On(ClientHandshake handshake)
    {
        try
        {
            handshake.ThrowIfInvalidProtocolIdentifier();
            agent.fsm.ChangeState<AfterClientHandshake>();
        }
        catch (ClientHandshake.InvalidClientHandshakeException)
        {
            Debug.Log("Invalid handshake. Kicking client.");
            KickClient();
        }
    }

    private void KickClient()
    {
        agent.connection.Close();
        Destroy(agent.gameObject);
    }
}