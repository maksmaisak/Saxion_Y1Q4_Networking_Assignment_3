﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

public class Checkerboard : IUnifiedSerializable
{
    public const int Size = 8;
    
    public enum TileState : byte
    {
        None,
        Black,
        White
    }
    
    static readonly Vector2Int[] DiagonalDirections =
    {
        new Vector2Int( 1,  1),
        new Vector2Int(-1,  1),
        new Vector2Int(-1, -1),
        new Vector2Int( 1, -1)
    };

    public delegate void PieceHandler(Checkerboard sender, Vector2Int position);
    public event PieceHandler OnPieceAdded;
    public event PieceHandler OnPieceRemoved;
    public event PieceHandler OnMultiCapture;
    
    public delegate void PieceMoveHandler(Checkerboard sender, Vector2Int origin, Vector2Int target);
    public event PieceMoveHandler OnPieceMoved;
    
    public TileState currentPlayerColor { get; private set; }
    public bool isDoingAMultiCapture { get; private set; }
    
    private TileState[,] tiles;
    public Vector2Int size { get; private set; }

    public Checkerboard()
    {
        currentPlayerColor = TileState.White;
        
        size = new Vector2Int(Size, Size);
        tiles = new TileState[size.x, size.y];
    }

    public Checkerboard(Checkerboard checkerboard)
    {
        currentPlayerColor = checkerboard.currentPlayerColor;
        size = checkerboard.size;
        tiles = (TileState[,])checkerboard.tiles.Clone();
    }
    
    public bool IsValidMove(Vector2Int origin, Vector2Int target)
    {
        if (IsOutOfBounds(origin)) return false;
        if (IsOutOfBounds(target)) return false;

        if (GetAt(origin) != currentPlayerColor) return false; // Wrong player's piece
        if (GetAt(target) != TileState.None) return false; // Target obstructed

        Vector2Int delta = target - origin;
        Vector2Int direction = new Vector2Int(Math.Sign(delta.x), Math.Sign(delta.y));
        Vector2Int absDelta  = new Vector2Int(      Abs(delta.x),       Abs(delta.y));

        if ((direction.y < 0) == (currentPlayerColor == TileState.White)) return false; // Wrong direction
        if (absDelta.x != absDelta.y) return false; // Not diagonal
        if (absDelta.x == 0) return false; // No move
        if (absDelta.x >  2) return false; // Too long

        if (absDelta.x == 2)
        {
            return GetAt(origin + direction) == GetOppositePlayer(currentPlayerColor); // Can't capture
        }

        if (isDoingAMultiCapture) return false; // Only captures are allowed during a multi-capture

        return true;
    }

    public bool IsCapturePossibleOutOf(Vector2Int origin)
    {
        Assert.IsFalse(IsOutOfBounds(origin));
        
        foreach (Vector2Int direction in DiagonalDirections)
        {
            Vector2Int afterCapturePos = origin + direction * 2;
            if (IsValidMove(origin, afterCapturePos)) return true;
        }

        return false;
    }
    
    /// Returns false if the move is invalid.
    public bool TryMakeMove(Vector2Int origin, Vector2Int target)
    {
        if (!IsValidMove(origin, target)) return false;

        Vector2Int delta = target - origin;
        int step = Abs(delta.x);
        
        if (step == 1) // No capture
        {
            Transport(origin, target);
        }
        else // Capture
        {
            Transport(origin, target);
            
            Vector2Int direction = new Vector2Int(Math.Sign(delta.x), Math.Sign(delta.y));
            RemovePieceAt(origin + direction);
            
            // Don't switch players if a multi-capture is possible
            if (IsCapturePossibleOutOf(target))
            {
                isDoingAMultiCapture = true;
                OnMultiCapture?.Invoke(this, target);
                return true;
            }
        }

        isDoingAMultiCapture = false;
        currentPlayerColor = GetOppositePlayer(currentPlayerColor);
        
        return true;
    }

    public TileState CheckVictory()
    {
        /*// TEMP Test
        if (Random.value < 0.25f)
        {
            return Random.value < 0.5f ? TileState.Black : TileState.White;
        }*/
        
        if (tiles.Cast<TileState>().All(t => t != TileState.Black)) return TileState.White;        
        if (tiles.Cast<TileState>().All(t => t != TileState.White)) return TileState.Black;

        return TileState.None;
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
        Assert.AreEqual(currentPlayerColor, GetAt(origin));
        
        var targets = new List<Vector2Int>();

        foreach (Vector2Int delta in DiagonalDirections)
        {
            Vector2Int adjacentPos = origin + delta;
            Vector2Int afterCapturePos = adjacentPos + delta;

            if (IsValidMove(origin, adjacentPos))
            {
                targets.Add(adjacentPos);
            }
            else if (IsValidMove(origin, afterCapturePos))
            {
                targets.Add(afterCapturePos);
            }
        }

        return targets;
    }
    
    public void AddPieceAt(Vector2Int position, TileState pieceColor)
    {
        Assert.IsFalse(IsOutOfBounds(position));
        Assert.AreEqual(TileState.None, GetAt(position));
        Assert.AreNotEqual(TileState.None, pieceColor);
        
        SetAt(position, pieceColor);
                
        OnPieceAdded?.Invoke(this, position);
    }
    
    public void Serialize(IUnifiedSerializer s)
    {
        byte value = (byte)currentPlayerColor;
        s.Serialize(ref value);
        currentPlayerColor = (TileState)value;

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

    private void RemovePieceAt(Vector2Int position)
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