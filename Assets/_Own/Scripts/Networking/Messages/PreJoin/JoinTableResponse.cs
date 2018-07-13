using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class JoinTableResponse : NetworkMessage<JoinTableResponse>
{
    public bool isReject;
    public string rejectionMessage;

    public static JoinTableResponse Accept => new JoinTableResponse {isReject = false};
    public static JoinTableResponse MakeReject(string rejectionMessage)
    {
        return new JoinTableResponse
        {
            isReject = true,
            rejectionMessage = rejectionMessage
        };
    }
        
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref isReject);

        if (isReject)
        {
            s.Serialize(ref rejectionMessage);
        }
    }
}