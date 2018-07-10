using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateInLobby : FsmState<Client>, 
    IEventReceiver<JoinTableResponse>
{
    [SerializeField] LobbyPanel lobbyPanel;

    private bool didSendJoinRequest;
    
    public override void Enter()
    {
        base.Enter();

        Assert.IsNotNull(lobbyPanel);
        lobbyPanel.EnableGUI();
        lobbyPanel.RegisterButtonJoinClickAction(OnJoinButtonClicked);
        lobbyPanel.SetStatusbarText("");

        didSendJoinRequest = false;
    }

    public override void Exit()
    {
        base.Exit();
        
        lobbyPanel.UnregisterButtonJoinClickActions();
        lobbyPanel.DisableGUI();
    }
    
    private void OnJoinButtonClicked()
    {
        if (didSendJoinRequest)
        {
            Debug.Log("Join chat request already set. Waiting");
            return;
        }
                
        agent.connectionToServer.Send(new JoinTableRequest());
        didSendJoinRequest = true;
        lobbyPanel.SetStatusbarText("Joining...");
    }

    public void On(JoinTableResponse request)
    {   
        if (request.isReject)
        {
            didSendJoinRequest = false;
            lobbyPanel.SetStatusbarText("Rejected. Reason: " + request.rejectionMessage);
            return;
        }

        agent.fsm.ChangeState<ClientStateInTable>();
    }
}