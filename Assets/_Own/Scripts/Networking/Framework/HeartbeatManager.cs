using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HeartbeatManager : MonoBehaviour
{
    [SerializeField] private Connection connection;
    [SerializeField] private float maxTimeIdleTillDisconnect = 10f;
    [SerializeField] private float maxTimeIdleTillHeartbeat  = 1f;

    void Start()
    {
        Assert.IsNotNull(connection);
    }

    void FixedUpdate()
    {
        if (connection.state != Connection.State.Running) return;
        
        if (connection.timeSinceLastReceive > maxTimeIdleTillDisconnect)
        {
            //Debug.Log(connection.timeSinceLastReceive + " Closing");
            connection.Close();
        }
        else if (connection.timeSinceLastSend > maxTimeIdleTillHeartbeat)
        {
            //Debug.Log(connection.timeSinceLastSend + " Sending heartbeat");
            connection.Send(new HeartbeatMessage());
        }
    }
}