using System;
using UnityEngine.Assertions;

/// Sent to a client who just joined the chatbox. Contains all the necessary state that needs to be displayed.
public class TableState : NetworkMessage<TableState>
{
    public string[] usernames;
    public string[] lines;
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref usernames);
        s.Serialize(ref lines);
    }
}
