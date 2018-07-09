using UnityEngine;

public class MakeMove : NetworkMessage<MakeMove>
{
    public Vector2Int origin;
    public Vector2Int destination;

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref origin);
        s.Serialize(ref destination);
    }
}