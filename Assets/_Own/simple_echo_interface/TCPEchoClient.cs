using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class TCPEchoClient : MonoBehaviour
{
    [SerializeField]
    private PanelWrapper _panelWrapper;
    [SerializeField]
    private string _hostname;
    [SerializeField]
    private int _port;

    //the tcpclient used to connect to the server
    private TcpClient _client = null;

    // Use this for initialization
    void Awake()
    {
        //process cmd line arguments
        _panelWrapper.RegisterInputHandler(onTextEntered);

        processCmdLineArgs();
        connectToServer();
    }

    private void processCmdLineArgs()
    {
        try
        {
            //get commandline args
            string[] cmdLineArgs = System.Environment.GetCommandLineArgs();
            //get only the pairs that have a : in them, split them on : into key,value 
            Dictionary<string, string> commands = 
                cmdLineArgs.
                Where(x => x.Contains(":")).
                Select(x=> x.Split(':')).
                ToDictionary(x => x[0], x => x[1]);

            _hostname = commands["server"];
            _port = int.Parse(commands["port"]);
        } catch {}
    }

    private void connectToServer()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(_hostname, _port);
            _panelWrapper.AddOutput("Connected to " + _client.Client.RemoteEndPoint);
            _panelWrapper.AddOutput("Connected from " + _client.Client.LocalEndPoint);
        }
        catch (Exception e)
        {
            _panelWrapper.AddOutput("Could not connect to " + _hostname + ":" + _port);
            _panelWrapper.AddOutput(e.Message);
            _client = null;
        }
    }

    private void onTextEntered()
    {
        if (_panelWrapper.GetInput() == "") return;

        try
        {
            Debug.Log("Sending " + _panelWrapper.GetInput());
            //StreamUtil.SendString(_client.GetStream(), _panelWrapper.GetInput(), Encoding.UTF8);
            _panelWrapper.ClearInput();

            //if we check for a reply over here, the server might not have responded yet and we will miss it
            //therefore we check each update whether a new message has arrived
            //this also allows us to send messages from the server to the client whenever we would like
        }
        catch (Exception e)
        {
            disconnect();
            _panelWrapper.AddOutput("Disconnected!:" + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_client == null || !_client.Connected) return;

        try
        {
            processServerData();
        }
        catch (Exception e)
        {
            disconnect();
            _panelWrapper.AddOutput("Disconnected!:" + e.Message);
        }
    }

    private void processServerData()
    {
        while (_client.Available > 0)
        {
            //string message = StreamUtil.ReceiveString(_client.GetStream(), Encoding.UTF8);
            //Debug.Log("Received:" + message);
            //if (message != null && message.Length > 0) _panelWrapper.AddOutput(message);
        }
    }

    private void disconnect()
    {
        if (_client == null) return;

        _client.Close();
        _client = null;
    }

    public void OnApplicationQuit()
    {
		_panelWrapper.UnregisterInputHandlers();
        disconnect();
    }

	private void OnDisable()
	{
		
	}
}

