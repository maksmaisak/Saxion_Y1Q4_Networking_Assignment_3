using UnityEngine;

public class MakeMove : NetworkMessage<MakeMove>
{
    public Vector2Int origin;
    public Vector2Int target;
    
    public MakeMove() {}

    public MakeMove(Vector2Int origin, Vector2Int target)
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