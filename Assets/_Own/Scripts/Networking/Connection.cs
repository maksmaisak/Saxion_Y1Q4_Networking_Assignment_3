using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine.Assertions;

public class Connection : MonoBehaviour
{
    public enum State
    {
        NotStarted,
        Running,
        Disconnected
    }
        
    private TcpClient client;
    private NetworkStream networkStream;
    private readonly ConcurrentQueue<INetworkMessage> messagesToSend = new ConcurrentQueue<INetworkMessage>();
        
    private bool isInitialized;

    public State state { get; private set; }

    public void Initialize(TcpClient client)
    {
        Assert.IsFalse(isInitialized, this + "is already initialized.");
        Assert.AreEqual(State.NotStarted, state, this + " has already been started!");
        Assert.IsNotNull(client);
        
        this.client = client;
        networkStream = client.GetStream();
        
        isInitialized = true;
        
        state = State.Running;
        new Thread(ReceivingThread) {IsBackground = true}.Start();
        new Thread(SendingThread  ) {IsBackground = true}.Start();
    }

    public void Send(INetworkMessage message)
    {
        Assert.AreEqual(State.Running, state, this + " is not running. Can't send message " + message);

        messagesToSend.Enqueue(message);
    }

    public void Close()
    {
        CloseConnection();
    }
    
    void OnDestroy()
    {
        CloseConnection();
    }

    private void ReceivingThread()
    {
        while (state == State.Running)
        {
            try
            {
                INetworkMessage message = NetworkMessageSerializer.Deserialize(networkStream);
                message.InitializeOnReceived(this);
                message.PostEvent();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception while deserializing a network message: " + ex);
                break;
            }
        }

        CloseConnection();
    }

    private void SendingThread()
    {
        while (state == State.Running)
        {
            while (messagesToSend.IsEmpty)
            {
                Thread.Sleep(10);
            } // TODO Use something better than a spinner like this.
            
            INetworkMessage message;
            while (messagesToSend.TryDequeue(out message))
            {
                Assert.IsNotNull(message);
                NetworkMessageSerializer.Serialize(message, networkStream);
            }
        }
        
        CloseConnection();
    }

    private void CloseConnection()
    {
        if (state != State.Running) return;

        lock (networkStream)
        {
            client.Close();
            networkStream.Close();
        }

        state = State.Disconnected;
    }
}