using UnityEngine;
using UnityEngine.Assertions;

public class JoinChatRequest : NetworkMessage<JoinChatRequest>
{
    public string nickname;

    public JoinChatRequest() {}

    public JoinChatRequest(string nickname)
    {
        this.nickname = nickname;
    }
    
    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref nickname);
    }
}