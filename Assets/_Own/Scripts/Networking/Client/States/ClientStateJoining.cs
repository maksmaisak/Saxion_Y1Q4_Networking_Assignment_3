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

    public void On(JoinServerResponse request)
    {
        Assert.IsTrue(isEntered);
        
        if (request.isReject)
        {
            connectPanel.SetStatusbarText("Rejected. Reason: " + request.rejectionMessage);
            // Don't change state here, expect Disconnect from the server to do it for you.
            return;
        }
        
        connectPanel.SetStatusbarText("Joined.");
        agent.fsm.ChangeState<ClientStateInLobby>();
    }
}