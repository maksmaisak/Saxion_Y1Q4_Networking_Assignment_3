using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class JoinChatResponse : NetworkMessage<JoinChatResponse>
{
    public bool isReject;
    public string rejectionMessage;

    public static JoinChatResponse Accept => new JoinChatResponse {isReject = false};
    public static JoinChatResponse MakeReject(string rejectionMessage)
    {
        return new JoinChatResponse
        {
            isReject = true,
            rejectionMessage = rejectionMessage
        };
    }

    public JoinChatResponse() {}
        
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref isReject);

        if (isReject)
        {
            s.Serialize(ref rejectionMessage);
        }
    }
}