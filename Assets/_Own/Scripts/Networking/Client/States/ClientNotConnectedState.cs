using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Assertions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class ClientNotConnectedState : FSMState<Client>
{
    [SerializeField] ConnectPanel connectPanel;
    [SerializeField] private Connection connectionPrefab;
    
    private CancellationTokenSource asyncCancellationTokenSource;
    private Task<TcpClient> connectionTask;
    
    public override void Enter()
    {
        base.Enter();
        
        connectPanel.EnableGUI();
        connectPanel.RegisterButtonConnectClickAction(OnConnectButtonClicked);
        connectPanel.SetStatusbarText("");
    }

    public override void Exit()
    {
        base.Exit();
        
        asyncCancellationTokenSource?.Cancel();
        
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

        if (connectionTask != null)
        {
            if (connectionTask.IsCanceled || connectionTask.IsFaulted)
            {
                connectionTask.Dispose();
                connectionTask = null;
            }
        }

        if (connectionTask == null) return;

        if (connectionTask.IsCompleted)
        {
            Debug.Log("Connection task complete.");

            asyncCancellationTokenSource.Dispose();
            asyncCancellationTokenSource = null;

            TcpClient connectedClient = connectionTask.Result;
            var connection = Instantiate(connectionPrefab, transform);
            connection.Initialize(connectedClient);

            agent.SetConnectionToServer(connection);
            
            agent.fsm.ChangeState<ClientChooseNameState>();
        }
        else
        {
            Debug.Log("Connection task pending...");
        }
    }

    private void OnConnectButtonClicked()
    {
        Debug.Log("OnConnectButtonClicked");

        if (connectionTask == null)
        {
            StartAsyncConnectionTask();
        }
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
}