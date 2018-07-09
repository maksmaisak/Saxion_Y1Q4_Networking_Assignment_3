using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.Mathf;

public class Checkerboard
{
    public const int Size = 10;
    
    public enum TileState
    {
        None,
        Black,
        White
    }

    public delegate void PieceHandler(Checkerboard sender, Vector2Int position);
    public event PieceHandler OnPieceRemoved;
    
    public delegate void PieceMoveHandler(Checkerboard sender, Vector2Int origin, Vector2Int target);
    public event PieceMoveHandler OnPieceMoved;

    private TileState currentPlayer;
    private readonly TileState[,] tiles;

    public Checkerboard()
    {
        currentPlayer = TileState.White;
        tiles = new TileState[Size, Size];
    }

    public bool TryMakeMove(Vector2Int origin, Vector2Int target)
    {
        if (IsOutOfBounds(origin)) return false;
        if (IsOutOfBounds(target)) return false;

        if (GetAt(origin) != currentPlayer) return false; // Wrong player's piece
        if (GetAt(target) != TileState.None) return false; // Target obstructed

        Vector2Int delta = target - origin;
        Vector2Int direction = new Vector2Int(Math.Sign(delta.x), Math.Sign(delta.y));
        Vector2Int absDelta  = new Vector2Int(      Abs(delta.x),       Abs(delta.y));
        
        if (absDelta.x != absDelta.y) return false; // Not diagonal
        if (absDelta.x == 0) return false; // No move
        if (absDelta.x > 2) return false; // Too long


        Vector2Int nextPosition = origin + direction;
        if (absDelta.x == 1) // No capture
        {
            SetAt(origin, TileState.None);
            SetAt(nextPosition, currentPlayer);
        }
        
        // Capture
        if (absDelta.x > 1 && GetAt(nextPosition) != GetOppositePlayer(currentPlayer)) return false;

        return true;
    }
    
    public bool IsValidMove(Vector2Int origin, Vector2Int target)
    {
        if (IsOutOfBounds(origin)) return false;
        if (IsOutOfBounds(target)) return false;

        if (GetAt(origin) != currentPlayer) return false; // Wrong player's piece
        if (GetAt(target) != TileState.None) return false; // Target obstructed

        Vector2Int delta = target - origin;
        Vector2Int direction = new Vector2Int(Math.Sign(delta.x), Math.Sign(delta.y));
        Vector2Int absDelta  = new Vector2Int(      Abs(delta.x),       Abs(delta.y));
        
        if (absDelta.x != absDelta.y) return false; // Not diagonal
        if (absDelta.x > 2) return false; // Too long

        Vector2Int nextPosition = origin + delta;

        if (absDelta.x == 1) // No capture
        {
            Transport(origin, nextPosition);
        }
        else // Capture
        {
            Vector2Int capturedPosition = origin + direction;
            if (absDelta.x > 1 && GetAt(capturedPosition) != GetOppositePlayer(currentPlayer)) return false;

            Transport(origin, nextPosition);
            RemoveAt(capturedPosition);
        }

        currentPlayer = GetOppositePlayer(currentPlayer);
        return true;
    }

    private void Transport(Vector2Int origin, Vector2Int destination)
    {
        SetAt(destination, GetAt(origin));
        SetAt(origin, TileState.None);
        
        OnPieceMoved?.Invoke(this, origin, destination);
    }

    private void RemoveAt(Vector2Int position)
    {
        SetAt(position, TileState.None);
        
        OnPieceRemoved?.Invoke(this, position);
    }

    private TileState GetOppositePlayer(TileState playerTileState)
    {
        Assert.AreNotEqual(TileState.None, playerTileState);
        return playerTileState == TileState.White ? TileState.Black : TileState.White;
    }

    private TileState GetAt(Vector2Int position)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        return tiles[position.x, position.y];
    }
    
    private void SetAt(Vector2Int position, TileState newState)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        tiles[position.x, position.y] = newState;
    }

    private bool IsOutOfBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x > tiles.GetLength(0)) return true;
        if (position.y < 0 || position.y > tiles.GetLength(1)) return true;

        return false;
    }
}