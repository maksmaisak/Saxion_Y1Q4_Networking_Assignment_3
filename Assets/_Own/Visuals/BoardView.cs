using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoardView : MonoBehaviour
{
    [SerializeField] GameObject blackTilePrefab;
    [SerializeField] GameObject whiteTilePrefab;
    [SerializeField] Transform tilesParent;
    [Space] 
    [SerializeField] GameObject blackCheckerPiecePrefab;
    [SerializeField] GameObject whiteCheckerPiecePrefab;
    [SerializeField] Transform piecesParent;
    
    private Checkerboard checkerboard;
    private GameObject[,] grid;

    void Start()
    {
        // Testing:
        
        SetCheckerboard(CheckersHelper.MakeDefaultCheckerboard());
    }
    
    public void SetCheckerboard(Checkerboard newCheckerboard)
    {
        Assert.IsNotNull(newCheckerboard);
        Assert.IsNull(checkerboard);
        
        checkerboard = newCheckerboard;

        InitializeGrid();
        checkerboard.OnPieceMoved   += CheckerboardOnPieceMoved;
        checkerboard.OnPieceRemoved += CheckerboardOnPieceRemoved;
    }
    
    public void Clear()
    {
        checkerboard.OnPieceMoved   -= CheckerboardOnPieceMoved;
        checkerboard.OnPieceRemoved -= CheckerboardOnPieceRemoved;
        checkerboard = null;
    }

    private void InitializeGrid()
    {
        Assert.IsNotNull(checkerboard);
        
        grid = new GameObject[checkerboard.size.x, checkerboard.size.y];

        for (int y = 0; y < checkerboard.size.y; ++y)
            for (int x = 0; x < checkerboard.size.x; ++x)
                GenerateTile(x, y);
    }

    private void GenerateTile(int x, int y)
    {
        var gridPosition = new Vector2Int(x, y); 
        
        Checkerboard.TileState tile = checkerboard.GetAt(gridPosition);

        AddTile(gridPosition);
        
        if (tile != Checkerboard.TileState.None)
        {
            grid[x, y] = AddPiece(gridPosition, isWhite: tile == Checkerboard.TileState.White);
        }
    }

    private GameObject AddTile(Vector2Int gridPosition)
    {
        bool isWhite = (gridPosition.x % 2 == 0) ^ (gridPosition.y % 2 == 0);
        GameObject prefab = isWhite ? whiteTilePrefab : blackTilePrefab;
        Assert.IsNotNull(prefab);

        GameObject tile = Instantiate(prefab, tilesParent);
        tile.transform.localPosition = GetLocalPositionForTileAt(gridPosition);
        return tile;
    }

    private GameObject AddPiece(Vector2Int gridPosition, bool isWhite)
    {
        GameObject prefab = isWhite ? whiteCheckerPiecePrefab : blackCheckerPiecePrefab;
        Assert.IsNotNull(prefab);
        
        GameObject piece = Instantiate(prefab, piecesParent);
        piece.transform.localPosition = GetLocalPositionForTileAt(gridPosition); 
        return piece;
    }

    private Vector3 GetLocalPositionForTileAt(Vector2Int gridPosition)
    {
        Vector2 tileSize = new Vector2(1f, 1f);
        
        Vector2 position = Vector2.Scale(tileSize, gridPosition);
        position -= Vector2.Scale(tileSize, checkerboard.size) * 0.5f;
        position += tileSize * 0.5f;

        return new Vector3(position.x, 0f, position.y);
    }

    private void CheckerboardOnPieceMoved(Checkerboard sender, Vector2Int origin, Vector2Int target)
    {
        throw new System.NotImplementedException();
    }
    
    private void CheckerboardOnPieceRemoved(Checkerboard sender, Vector2Int position)
    {
        throw new System.NotImplementedException();
    }
}
