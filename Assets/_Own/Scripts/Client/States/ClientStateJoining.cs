using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateJoining : FsmState<Client>, 
    IEventReceiver<JoinServerResponse>
{
    [SerializeField] ConnectPanel connectPanel;

    public override void Enter()
    {
        base.Enter();
        
        Assert.IsNotNull(connectPanel);
        connectPanel.SetStatusbarText("Joining server...");
    }

    public void On(JoinServerResponse response)
    {
        Assert.IsTrue(isEntered);
        
        if (response.isReject)
        {
            connectPanel.SetStatusbarText("Rejected. Reason: " + response.rejectionMessage);
            // Don't change state here, expect Disconnect from the server to do it for you.
            return;
        }
        
        connectPanel.SetStatusbarText("");
        agent.playerId = response.playerId;
       
        agent.fsm.ChangeState<ClientStateInLobby>();
    }
}