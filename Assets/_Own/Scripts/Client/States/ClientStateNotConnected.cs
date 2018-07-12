using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Assertions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class ClientStateNotConnected : FsmState<Client>
{
    [SerializeField] ConnectPanel connectPanel;
    [SerializeField] private Connection connectionPrefab;
    
    private CancellationTokenSource asyncCancellationTokenSource;
    private Task<TcpClient> connectionTask;
    
    public override void Enter()
    {
        base.Enter();

        agent.playerInfo = new PlayerInfo();
        
        connectPanel.EnableGUI();
        connectPanel.RegisterButtonConnectClickAction(OnConnectButtonClicked);
    }

    public override void Exit()
    {
        base.Exit();
        
        asyncCancellationTokenSource?.Cancel();
        asyncCancellationTokenSource = null;
        connectionTask = null;
        
        connectPanel.UnregisterButtonConnectClickActions();
        connectPanel.DisableGUI();
    }

    protected override void OnDestroy()
    {
        asyncCancellationTokenSource?.Cancel();
    }

    void Update()
    {
        Assert.IsNotNull(connectionPrefab);
        
        if (connectionTask == null) return;

        if (connectionTask.IsCanceled || connectionTask.IsFaulted)
        {
            connectionTask.Dispose();
            connectionTask = null;
            return;
        }

        if (!connectionTask.IsCompleted)
        {
            connectPanel.SetStatusbarText("Connecting...");
            Debug.Log("Connection task pending...");
            return;
        }

        OnConnectionTaskComplete();
    }

    private void OnConnectButtonClicked()
    {
        Debug.Log("OnConnectButtonClicked");
        if (connectionTask != null) return;
        if (!connectPanel.Validate()) return;

        agent.playerInfo.nickname = connectPanel.GetNickname();
        StartAsyncConnectionTask();
    }

    private void StartAsyncConnectionTask()
    {
        asyncCancellationTokenSource = new CancellationTokenSource();

        var ipAddress = IPAddress.Parse(connectPanel.GetServerName());
        int portNumber = connectPanel.GetPort();
        var remoteEndPoint = new IPEndPoint(ipAddress, portNumber);
        connectionTask = TryConnectAsync(remoteEndPoint, asyncCancellationTokenSource.Token);
    }
    
    private static async Task<TcpClient> TryConnectAsync(IPEndPoint remoteEndPoint, CancellationToken cancellationToken)
    {
        TcpClient client = null;
        
        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                client = new TcpClient();
                await client.ConnectAsync(remoteEndPoint.Address, remoteEndPoint.Port);
                Assert.IsTrue(client.Connected);

                cancellationToken.ThrowIfCancellationRequested();
               
                Debug.Log("Connect successful.");
                return client;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Connect cancelled.");
                client?.Close();
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log("Couldn't connect. Exception: " + ex);
                client?.Close();
            }
        }
    }
    
    private void OnConnectionTaskComplete()
    {
        connectPanel.SetStatusbarText("Connected.");
        Debug.Log("Connection task complete.");

        asyncCancellationTokenSource.Dispose();
        asyncCancellationTokenSource = null;

        Connection connection = MakeConnection(connectionTask.Result);
        agent.SetConnectionToServer(connection);
        agent.connectionToServer.Send(new JoinServerRequest(agent.playerInfo.nickname));
        
        agent.fsm.ChangeState<ClientStateJoining>();
    }

    private Connection MakeConnection(TcpClient connectedClient)
    {
        Connection connection = Instantiate(connectionPrefab, transform);
        connection.gameObject.name = $"ClientSideConnection ({connectedClient.Client.RemoteEndPoint})";
        connection.Initialize(connectedClient);

        return connection;
    }
}