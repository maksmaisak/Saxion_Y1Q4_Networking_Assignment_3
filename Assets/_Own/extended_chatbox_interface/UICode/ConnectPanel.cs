using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class ConnectPanel : BasePanel {

    [SerializeField]
    private Button _buttonConnect;

    [SerializeField]
    private Text _statusBar;

    [SerializeField]
    private InputField _entryServer;

    [SerializeField]
    private InputField _entryPort;

    public string GetServerName() {
		return _entryServer.text;
	}

	public int GetPort() {
		return int.Parse (_entryPort.text);
	}

	public bool Validate() {
		try {
			int.Parse (_entryPort.text);
		} catch {
            _statusBar.text = "Please enter a valid port";
			return false;
		}

		if (_entryServer.text.Trim().Length == 0) {
            _statusBar.text = "Please enter a server";
			return false;
		}

		return true;
	}

    public void RegisterButtonConnectClickAction (UnityAction pAction)
    {
        _buttonConnect.onClick.AddListener(pAction);
    }

    public void UnregisterButtonConnectClickActions ()
    {
        _buttonConnect.onClick.RemoveAllListeners();
    }

    public void SetStatusbarText(string pInfo)
    {
        _statusBar.text = pInfo;
    }

}
