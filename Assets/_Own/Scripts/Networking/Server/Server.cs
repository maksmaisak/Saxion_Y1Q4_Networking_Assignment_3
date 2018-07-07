using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Assertions;
using UnityEngine;

public class Server : Singleton<Server>
{
    [SerializeField] int portNumber = 55555;
    [SerializeField] Connection connectionPrefab;
    
    public ChatboxState state { get; private set; }
    
    private readonly ConcurrentQueue<TcpClient> pendingConnectedTcpClients = new ConcurrentQueue<TcpClient>();
    // TODO Remove closed connections frome here.
    private readonly LinkedList<Connection> connections = new LinkedList<Connection>();

    private Thread listeningThread;
    
    void Start()
    {
        Assert.IsNotNull(connectionPrefab);
        
        state = new ChatboxState();

        listeningThread = new Thread(Listen) {IsBackground = true};
        listeningThread.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        listeningThread?.Abort();
    }

    void Update()
    {
        TcpClient client;
        while (pendingConnectedTcpClients.TryDequeue(out client))
        {
            Debug.Log("Starting a new Connection.");
            
            Connection connection = Instantiate(connectionPrefab, transform);
            connection.gameObject.name = $"ServerSideConnection ({client.Client.RemoteEndPoint})";

            connection.Initialize(client);

            connections.AddLast(connection);
            new NewConnection(connection).PostEvent();
        }
    }
    
    public void SendAllClients(INetworkMessage message)
    {
        var toRemove = connections.Where(c => !c || c == null || c.state == Connection.State.Closed);
        foreach (Connection connection in toRemove)
        {
            connections.Remove(connection);
        }

        foreach (Connection connection in connections)
        {
            connection.Send(message);
        }
    }

    private void Listen()
    {
        var listener = new TcpListener(IPAddress.Any, portNumber);
        listener.Start();

        while (true)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                pendingConnectedTcpClients.Enqueue(client);

                Debug.Log("New client connected.");
            }
            catch (ThreadAbortException)
            {
                Debug.Log("Server listening thread aborted.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Unforseen exception: " + ex);
            }
        }
    }
}