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
    [SerializeField] Table tablePrefab;
        
    private readonly ConcurrentQueue<TcpClient> pendingConnectedTcpClients = new ConcurrentQueue<TcpClient>();
    public readonly HashSet<ServerPlayer> joinedPlayers = new HashSet<ServerPlayer>();

    public Table table { get; private set; }
    
    private Thread listeningThread;

    private uint nextPlayerId;
    
    void Start()
    {
        Assert.IsNotNull(connectionPrefab);
        Assert.IsNotNull(tablePrefab);

        table = Instantiate(tablePrefab, transform);
        
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

            new NewConnection(connection).PostEvent();
        }
    }
    
    public uint GetNextPlayerId()
    {
        return nextPlayerId++;
    }
    
    public void SendAllConnectedPlayers(INetworkMessage message)
    {
        foreach (ServerPlayer player in joinedPlayers)
        {
            player.connection.Send(message);
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