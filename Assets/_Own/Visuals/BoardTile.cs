using System;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public event Action<BoardTile> OnClick;

    void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }
}