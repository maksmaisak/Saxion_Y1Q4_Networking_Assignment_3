using UnityEngine;

public class ClientChooseNameState : FsmState<Client>, 
    IEventReceiver<JoinChatResponse>
{
    [SerializeField] NameChoosePanel nameChoosePanel;

    private bool didSendJoinRequest;
    
    public override void Enter()
    {
        base.Enter();

        nameChoosePanel.EnableGUI();
        nameChoosePanel.RegisterButtonJoinClickAction(OnJoinButtonClicked);
        nameChoosePanel.SetStatusbarText("");
    }

    public override void Exit()
    {
        base.Exit();
        
        nameChoosePanel.UnregisterButtonJoinClickActions();
        nameChoosePanel.DisableGUI();
    }
    
    private void OnJoinButtonClicked()
    {
        if (didSendJoinRequest)
        {
            Debug.Log("Join chat request already set. Waiting");
            return;
        }
                
        string nickname = nameChoosePanel.GetNickName();
        var message = new JoinChatRequest(nickname);
        agent.connectionToServer.Send(message);
        
        didSendJoinRequest = true;
        nameChoosePanel.SetStatusbarText("Joining...");
    }

    public void On(JoinChatResponse response)
    {   
        if (response.isReject)
        {
            didSendJoinRequest = false;
            nameChoosePanel.SetStatusbarText("Rejected. Reason: " + response.rejectionMessage);
            return;
        }

        agent.fsm.ChangeState<ClientInChatState>();
    }
}