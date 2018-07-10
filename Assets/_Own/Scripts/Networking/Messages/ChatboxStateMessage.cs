using System;
using UnityEngine.Assertions;

/// Sent to a client who just joined the chatbox. Contains all the necessary state that needs to be displayed.
public class ChatboxStateMessage : NetworkMessage<ChatboxStateMessage>
{
    public string[] nicknames;
    public string[] lines;
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nicknames);
        s.Serialize(ref lines);
    }
}
