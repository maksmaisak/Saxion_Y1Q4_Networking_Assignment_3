using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;

public class Server : Singleton<Server>
{
    [SerializeField] int portNumber = 55555;
    [SerializeField] Connection connectionPrefab;

    private readonly ConcurrentQueue<TcpClient> pendingConnectedTcpClients = new ConcurrentQueue<TcpClient>();
    // TODO Remove closed connections frome here.
    private readonly LinkedList<Connection> connections = new LinkedList<Connection>();

    void Start()
    {
        Assert.IsNotNull(connectionPrefab);

        var thread = new Thread(Listen) {IsBackground = true};
        thread.Start();
    }

    void Update()
    {
        TcpClient client;
        while (pendingConnectedTcpClients.TryDequeue(out client))
        {
            Debug.Log("Starting a new Connection.");
            
            Connection connection = Instantiate(connectionPrefab, transform);
            connection.Initialize(client);

            connections.AddLast(connection);
            new NewConnection(connection).PostEvent();
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
            catch (Exception ex)
            {
                Debug.LogError("Unforseen exception: " + ex);
            }
        }
    }
}