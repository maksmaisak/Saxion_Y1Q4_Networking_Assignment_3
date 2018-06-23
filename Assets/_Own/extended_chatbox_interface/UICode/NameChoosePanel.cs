using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class NameChoosePanel : BasePanel {

    [SerializeField]
    private Button _buttonJoin;

    [SerializeField]
    private Text _statusBar;

    [SerializeField]
    private InputField _entryNickname;

    public string GetNickName()
    {
        return _entryNickname.text;
    }

    public bool Validate()
    {
        if (_entryNickname.text.Trim().Length == 0)
        {
            _statusBar.text = "Please enter a nickname";
            return false;
        }

        return true;
    }

    public void RegisterButtonJoinClickAction(UnityAction pAction)
    {
        _buttonJoin.onClick.AddListener(pAction);
    }

    public void UnregisterButtonJoinClickActions()
    {
        _buttonJoin.onClick.RemoveAllListeners();
    }

    public void SetStatusbarText(string pInfo)
    {
        _statusBar.text = pInfo;
    }


}
