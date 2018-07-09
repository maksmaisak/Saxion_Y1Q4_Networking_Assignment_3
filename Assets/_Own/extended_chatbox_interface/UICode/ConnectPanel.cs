using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Net;
using UnityEngine.Events;

public class ConnectPanel : BasePanel
{
    [SerializeField] private Button buttonConnect;
    [SerializeField] private Text   statusBar;
    [SerializeField] private InputField entryServer;
    [SerializeField] private InputField entryPort;
    [SerializeField] private InputField entryNickname;

    public string GetNickname()
    {
        return entryNickname.text;
    }

    public string GetServerName()
    {
        return entryServer.text;
    }

    public int GetPort()
    {
        return int.Parse(entryPort.text);
    }

    public bool Validate()
    {
        try
        {
            int.Parse(entryPort.text);
        }
        catch
        {
            statusBar.text = "Please enter a valid port";
            return false;
        }

        if (entryServer.text.Trim().Length == 0)
        {
            statusBar.text = "Please enter the IP-address of a server";
            return false;
        }

        return true;
    }

    public void RegisterButtonConnectClickAction(UnityAction pAction)
    {
        buttonConnect.onClick.AddListener(pAction);
    }

    public void UnregisterButtonConnectClickActions()
    {
        buttonConnect.onClick.RemoveAllListeners();
    }

    public void SetStatusbarText(string pInfo)
    {
        statusBar.text = pInfo;
    }
}