using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateJoining : FsmState<Client>, 
    IEventReceiver<JoinServerResponse>,
    IEventReceiver<DisconnectMessage>
{
    [SerializeField] ConnectPanel connectPanel;

    private bool didReceiveResponse;

    public override void Enter()
    {
        base.Enter();

        didReceiveResponse = false;
        
        Assert.IsNotNull(connectPanel);
        connectPanel.SetStatusbarText("Joining server...");
    }

    public override void Exit()
    {
        base.Exit();

        if (!didReceiveResponse)
        {
            connectPanel.SetStatusbarText("Connection interrupted.");
        }
    }

    public void On(JoinServerResponse response)
    {
        Assert.IsTrue(isEntered);
        didReceiveResponse = true;
        
        if (response.isReject)
        {
            connectPanel.SetStatusbarText("<color=red>Rejected</color>. Reason: " + response.rejectionMessage);
            // Don't change state here, expect the DisconnectMessage from the server to do it for you.
            return;
        }
        
        connectPanel.SetStatusbarText("");
        agent.playerInfo.id = response.playerId;
       
        agent.fsm.ChangeState<ClientStateInLobby>();
    }

    public void On(DisconnectMessage message)
    {
        if (!isEntered) return;
        
        if (didReceiveResponse) return;
        didReceiveResponse = true;
        
        connectPanel.SetStatusbarText("<color=red>Rejected</color>. Reason unknown.");
    }
}