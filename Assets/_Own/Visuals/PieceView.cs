using System;
using UnityEngine;
using DG.Tweening;

public class PieceView : MonoBehaviour
{
    [SerializeField] float moveDuration = 0.2f;
    [SerializeField] float moveJumpHeight = 0.2f;
    [Space]
    [SerializeField] float disappearDuration = 0.2f;
    
    public event Action<PieceView> OnClick;
    
    public Vector2Int gridPosition { get; set; }

    void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }

    public void MoveTo(Vector3 targetLocalPosition)
    {
        transform
            .DOLocalJump(targetLocalPosition, moveJumpHeight, 1, moveDuration)
            .SetEase(Ease.InOutQuart);
    }

    public void Capture()
    {
        transform
            .DOScale(Vector3.zero, disappearDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}
