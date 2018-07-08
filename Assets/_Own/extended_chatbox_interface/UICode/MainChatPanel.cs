using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class MainChatPanel : BasePanel {

    [SerializeField]
    private ScrollRect _scrollView;
    [SerializeField]
    private UserButton _userButtonPrefab;
    [SerializeField]
    private GameObject _userParent;
    [SerializeField]
    private Text _chatText;
    [SerializeField]
    private InputField _chatEntry;
    [SerializeField]
    private Button _send;
    [SerializeField]
    private Button _disconnect;

    private Dictionary<string, UserButton> _userButtonMap = new Dictionary<string, UserButton>();

    public delegate void OnUserClickedHandler(string pUser);
    public event OnUserClickedHandler OnUserClicked;

    private bool _focusedRequested = true;
    private bool _scrollRequested = true;

    public void AddUser(string pUserName)
    {
       UserButton user = Instantiate(_userButtonPrefab);
       user.onClick.AddListener(() => buttonClicked(pUserName));
       user.SetName(pUserName);
       user.transform.SetParent(_userParent.transform,false);
       _userButtonMap[pUserName] = user;
    }

    public void RemoveAllUsers()
    {
        List<string> users = _userButtonMap.Keys.ToList();

        for (int i = 0; i < users.Count; i++)
        {
            string name = users[i];
            UserButton button = _userButtonMap[name];
            button.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
        }
        
        _userButtonMap.Clear();
    }

    public void RemoveUser(string pUserName)
    {
        if (_userButtonMap.ContainsKey(pUserName))
        {
            UserButton button = _userButtonMap[pUserName];
            _userButtonMap.Remove(pUserName);
            button.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
        }
    }

    private void buttonClicked (string pUserName)
    {
        Debug.Log(pUserName);
        if (OnUserClicked != null) OnUserClicked(pUserName);
    }

    public string GetChatEntry()
    {
        return _chatEntry.text;
    }

    public void SetChatEntry(string pEntry)
    {
        _chatEntry.text = pEntry;
        _focusedRequested = true;
    }

    public void AddChatLine(string pChatLine)
    {
        _chatText.text += pChatLine + "\n";
        _scrollView.verticalNormalizedPosition = 0;
    }

    public void ClearAllText ()
    {
        _chatText.text = "";
    }

    public void RegisterButtonSendClickAction(UnityAction pAction)
    {
        _send.onClick.AddListener(pAction);
        _chatEntry.onEndEdit.AddListener(value => pAction());
        _focusedRequested = true;
    }

    public void RegisterButtonDisconnectClickAction(UnityAction pAction)
    {
        _disconnect.onClick.AddListener(pAction);
    }

    public void UnregisterButtonSendClickActions()
    {
        _send.onClick.RemoveAllListeners();
        _chatEntry.onEndEdit.RemoveAllListeners();
    }

    public void UnregisterButtonDisconnectClickActions()
    {
        _disconnect.onClick.RemoveAllListeners();
    }

    public void Update()
    {
        if (_focusedRequested)
        {
            _chatEntry.ActivateInputField();
            _chatEntry.Select();
            _focusedRequested = false;
        }
    }

}
