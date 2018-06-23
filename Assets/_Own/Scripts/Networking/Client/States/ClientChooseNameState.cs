using UnityEngine;

public class ClientChooseNameState : FSMState<Client>
{
    [SerializeField] NameChoosePanel nameChoosePanel;
    
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
        Debug.Log("OnJoinButtonClicked");
        
        agent.fsm.ChangeState<ClientNotConnectedState>();
    }
}