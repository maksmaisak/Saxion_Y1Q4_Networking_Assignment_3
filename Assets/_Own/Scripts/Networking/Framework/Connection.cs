using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

public class Connection : MonoBehaviour
{
    public enum State
    {
        NotStarted,
        Running,
        Closing, // Close requested, but still some messages in the queue need to be pushed into the network stream.
        Closed
    }
    
    public delegate void NetworkMessageHandler(Connection source, INetworkMessage message);
    public event NetworkMessageHandler OnMessageReceived;
    
    public State state { get; private set; }
    public float timeOfLastReceive { get; private set; }
    public float timeSinceLastReceive => Time.time - timeOfLastReceive;
    
    private TcpClient client;
    private NetworkStream networkStream;
    private readonly ConcurrentQueue<INetworkMessage> messagesToSend    = new ConcurrentQueue<INetworkMessage>();
    private readonly ConcurrentQueue<INetworkMessage> messagesToProcess = new ConcurrentQueue<INetworkMessage>();

    private readonly object locker = new object();
    
    private bool isInitialized;
    private bool didReceiveSinceLastUpdate;

    private Thread receivingThread;
    private Thread sendingThread;

    public void Initialize(TcpClient connectedClient)
    {
        Assert.IsFalse(isInitialized, this + "is already initialized.");
        Assert.AreEqual(State.NotStarted, state, this + " has already been started!");
        Assert.IsNotNull(connectedClient);
        
        client = connectedClient;
        networkStream = connectedClient.GetStream();
        
        isInitialized = true;
        
        timeOfLastReceive = Time.time;
        state = State.Running;
        
        receivingThread = new Thread(ReceivingThread) {IsBackground = true};
        receivingThread.Start();
        
        sendingThread = new Thread(SendingThread) {IsBackground = true};
        sendingThread.Start();
    }

    public void Send(INetworkMessage message)
    {
        Assert.AreEqual(State.Running, state, this + " is not running. Can't send message " + message);

        messagesToSend.Enqueue(message);
    }

    public void Close()
    {
        lock (locker)
        {
            if (state == State.Closing || state == State.Closed) return;
            state = State.Closing;
        }
    }
    
    void OnDestroy()
    {
        Close();
    }
    
    void FixedUpdate()
    {
        if (didReceiveSinceLastUpdate)
        {
            timeOfLastReceive = Time.time;
            didReceiveSinceLastUpdate = false;
        }

        INetworkMessage message;
        while (messagesToProcess.TryDequeue(out message))
        {
            OnMessageReceived?.Invoke(this, message);
        }
    }

    private void ReceivingThread()
    {
        while (state == State.Running)
        {
            try
            {
                INetworkMessage message = NetworkMessageSerializer.Deserialize(networkStream);
                message.InitializeOnReceived(this);
                Debug.Log("Received " + message);

                messagesToProcess.Enqueue(message);
                message.PostEvent();

                didReceiveSinceLastUpdate = true;
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.Log("Exception while deserializing a network message: " + ex);
                Close();
            }
        }
    }

    private void SendingThread()
    {
        while (state == State.Running)
        {
            // TODO Use something better than a spinner like this.
            while (state == State.Running && messagesToSend.IsEmpty)
            {
                Thread.Sleep(10);
            }

            SendAllMessagesInQueue();
        }

        if (state == State.Closing)
        {
            SendAllMessagesInQueue();
            CloseClient();
        }
    }

    private void CloseClient()
    {
        if (state != State.Running && state != State.Closing) return;
        
        receivingThread.Abort();
        
        client.Close();
        networkStream.Close();

        state = State.Closed;
    }

    private void SendAllMessagesInQueue()
    {
        INetworkMessage message;
        while (messagesToSend.TryDequeue(out message))
        {
            Assert.IsNotNull(message);
            Debug.Log("Sending " + message);

            try
            {
                NetworkMessageSerializer.Serialize(message, networkStream);
            }
            catch (Exception ex)
            {
                Debug.Log("Exception while serializing a network message: " + ex);
                Close();
                break;
            }
        }   
    }
}