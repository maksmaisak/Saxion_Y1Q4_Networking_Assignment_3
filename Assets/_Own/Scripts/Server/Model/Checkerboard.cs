using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.Mathf;

public class Checkerboard : IUnifiedSerializable
{
    public const int Size = 8;
    
    public enum TileState : byte
    {
        None,
        Black,
        White
    }

    public delegate void PieceHandler(Checkerboard sender, Vector2Int position);
    public event PieceHandler OnPieceAdded;
    public event PieceHandler OnPieceRemoved;
    
    public delegate void PieceMoveHandler(Checkerboard sender, Vector2Int origin, Vector2Int target);
    public event PieceMoveHandler OnPieceMoved;

    public TileState currentPlayer { get; private set; }
    
    private TileState[,] tiles;
    public Vector2Int size { get; private set; }

    public Checkerboard()
    {
        currentPlayer = TileState.White;
        
        size = new Vector2Int(Size, Size);
        tiles = new TileState[size.x, size.y];
    }

    public Checkerboard(Checkerboard checkerboard)
    {
        currentPlayer = checkerboard.currentPlayer;
        size = checkerboard.size;
        tiles = (TileState[,])checkerboard.tiles.Clone();
    }
    
    /// Returns false if the move is invalid.
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
        if (absDelta.x >  2) return false; // Too long

        if (absDelta.x == 1) // No capture
        {
            Transport(origin, target);
        }
        else // Capture
        {
            Vector2Int capturedPosition = origin + direction;
            if (absDelta.x > 1 && GetAt(capturedPosition) != GetOppositePlayer(currentPlayer)) return false;

            Transport(origin, target);
            RemoveAt(capturedPosition);
        }

        currentPlayer = GetOppositePlayer(currentPlayer);
        return true;
    }
    
    public TileState GetAt(Vector2Int position)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        return tiles[position.x, position.y];
    }
    
    public TileState GetAt(int x, int y)
    {
        Assert.IsFalse(IsOutOfBounds(new Vector2Int(x, y)));
        return tiles[x, y];
    }
    
    public IReadOnlyCollection<Vector2Int> GetValidMoveDestinations(Vector2Int origin)
    {
        Assert.IsFalse(IsOutOfBounds(origin));
        Assert.AreEqual(currentPlayer, GetAt(origin));

        Vector2Int[] deltas =
        {
            new Vector2Int( 1,  1),
            new Vector2Int(-1,  1),
            new Vector2Int(-1, -1),
            new Vector2Int( 1, -1)
        };
        
        var targets = new List<Vector2Int>();

        foreach (Vector2Int delta in deltas)
        {
            Vector2Int adjacentPos = origin + delta;
            Vector2Int capturePos = adjacentPos + delta;
            if (!IsOutOfBounds(adjacentPos) && GetAt(adjacentPos) == TileState.None)
            {
                targets.Add(adjacentPos);
            }
            else if (!IsOutOfBounds(capturePos) && GetAt(adjacentPos) == GetOppositePlayer(currentPlayer))
            {
                targets.Add(capturePos);
            }
        }

        return targets;
    }
    
    public void AddAt(Vector2Int position, TileState pieceColor)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        Assert.AreEqual(TileState.None, GetAt(position));
        Assert.AreNotEqual(TileState.None, pieceColor);
        
        SetAt(position, pieceColor);
                
        OnPieceAdded?.Invoke(this, position);
    }
    
    public void Serialize(IUnifiedSerializer s)
    {
        byte value = (byte)currentPlayer;
        s.Serialize(ref value);
        currentPlayer = (TileState)value;

        Vector2Int sizeValue = size;
        s.Serialize(ref sizeValue);
        size = sizeValue; 

        SerializeTiles(s);
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

    private static TileState GetOppositePlayer(TileState playerTileState)
    {
        Assert.AreNotEqual(TileState.None, playerTileState);
        return playerTileState == TileState.White ? TileState.Black : TileState.White;
    }

    private void SetAt(Vector2Int position, TileState newState)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        tiles[position.x, position.y] = newState;
    }

    private bool IsOutOfBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x >= size.x) return true;
        if (position.y < 0 || position.y >= size.y) return true;

        return false;
    }
    
    private void SerializeTiles(IUnifiedSerializer s)
    {   
        if (s.isReading)
        {
            tiles = new TileState[size.x, size.y];
        }

        for (int y = 0; y < size.y; ++y)
        {
            for (int x = 0; x < size.x; ++x)
            {
                byte value = (byte)tiles[x, y];
                s.Serialize(ref value);
                tiles[x, y] = (TileState)value;
            }
        }
    }
}