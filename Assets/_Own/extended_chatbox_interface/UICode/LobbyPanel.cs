using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class LobbyPanel : BasePanel
{
    [SerializeField] private Button buttonJoin;
    [SerializeField] private Text   statusBar;

    public void RegisterButtonJoinClickAction(UnityAction pAction)
    {
        buttonJoin.onClick.AddListener(pAction);
    }

    public void UnregisterButtonJoinClickActions()
    {
        buttonJoin.onClick.RemoveAllListeners();
    }

    public void SetStatusbarText(string pInfo)
    {
        statusBar.text = pInfo;
    }
}