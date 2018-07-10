using UnityEngine;

public class NotifyMakeMove : NetworkMessage<NotifyMakeMove>
{
    public Vector2Int origin;
    public Vector2Int target;
    
    public NotifyMakeMove() {}

    public NotifyMakeMove(Vector2Int origin, Vector2Int target)
    {
        this.origin = origin;
        this.target = target;
    }

    public override void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref origin);
        s.Serialize(ref target);
    }
}