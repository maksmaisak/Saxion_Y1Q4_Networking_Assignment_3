using System;
using UnityEngine;

public class PieceView : MonoBehaviour
{
    public event Action<PieceView> OnClick;

    void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }
}
