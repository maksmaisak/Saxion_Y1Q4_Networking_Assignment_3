using System;
using UnityEngine;

public class PieceView : MonoBehaviour
{
    public event Action<PieceView> OnClick;
    
    public Vector2Int gridPosition { get; set; }

    void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }

    public void MoveTo(Vector3 targetLocalPosition)
    {
        transform.localPosition = targetLocalPosition;
    }

    public void Capture()
    {
        Destroy(gameObject);
    }
}
