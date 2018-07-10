using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoardView : MonoBehaviour
{
    [SerializeField] BoardTile blackTilePrefab;
    [SerializeField] BoardTile whiteTilePrefab;
    [SerializeField] Transform tilesParent;
    [Space] 
    [SerializeField] PieceView blackCheckerPiecePrefab;
    [SerializeField] PieceView whiteCheckerPiecePrefab;
    [SerializeField] Transform piecesParent;
    [Space] 
    [SerializeField] ColorOverlay overlayPrefab;
    [SerializeField] Transform overlaysParent;

    public delegate void OnMoveRequestHandler(Vector2Int originPosition, Vector2Int destinationPosition);
    public event OnMoveRequestHandler OnMoveRequest;
    
    private Checkerboard checkerboard;
    private GridCell[,] grid;

    private Vector2Int? currentlySelectedPosition;
    
    private struct GridCell
    {
        public BoardTile tile;
        public ColorOverlay overlay;
        public PieceView pieceView;
    }
    
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
        
        throw new NotImplementedException();
        // TODO Remove existing pieces, tiles, and overlays.
    }

    private void InitializeGrid()
    {
        Assert.IsNotNull(checkerboard);
        
        grid = new GridCell[checkerboard.size.x, checkerboard.size.y];

        for (int y = 0; y < checkerboard.size.y; ++y)
            for (int x = 0; x < checkerboard.size.x; ++x)
                GenerateTileView(x, y);
    }

    private void GenerateTileView(int x, int y)
    {
        var gridPosition = new Vector2Int(x, y); 
        
        Checkerboard.TileState tile = checkerboard.GetAt(gridPosition);

        grid[x, y].tile    = AddTile(gridPosition);
        grid[x, y].overlay = AddOverlay(gridPosition);
        
        if (tile != Checkerboard.TileState.None)
        {
            grid[x, y].pieceView = AddPiece(gridPosition, isWhite: tile == Checkerboard.TileState.White);
        }
    }

    private BoardTile AddTile(Vector2Int gridPosition)
    {
        bool isWhite = (gridPosition.x % 2 == 0) ^ (gridPosition.y % 2 == 0);
        BoardTile prefab = isWhite ? whiteTilePrefab : blackTilePrefab;
        Assert.IsNotNull(prefab);

        BoardTile tile = Instantiate(prefab, tilesParent);
        tile.transform.localPosition = GetLocalPositionForTileAt(gridPosition);
        return tile;
    }

    private ColorOverlay AddOverlay(Vector2Int gridPosition)
    {
        Assert.IsNotNull(overlayPrefab); 
        
        ColorOverlay overlay = Instantiate(overlayPrefab, overlaysParent);
        overlay.transform.localPosition = GetLocalPositionForTileAt(gridPosition);
        return overlay;
    }

    private PieceView AddPiece(Vector2Int gridPosition, bool isWhite)
    {        
        PieceView prefab = isWhite ? whiteCheckerPiecePrefab : blackCheckerPiecePrefab;
        Assert.IsNotNull(prefab);
        
        PieceView piece = Instantiate(prefab, piecesParent);
        piece.transform.localPosition = GetLocalPositionForTileAt(gridPosition);
        piece.OnClick += (sender) => PieceViewOnClick(sender, gridPosition);
        
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

    private void PieceViewOnClick(PieceView sender, Vector2Int gridPosition)
    {
        GridCell gridCell = grid[gridPosition.x, gridPosition.y];

        if (checkerboard.GetAt(gridPosition) != checkerboard.currentPlayer) return;
        
        if (currentlySelectedPosition.HasValue)
        {
            if (currentlySelectedPosition == gridPosition) return;
            TurnOffAllOverlays();
            currentlySelectedPosition = null;
        }

        gridCell.overlay.SetActive(true);
        foreach (Vector2Int position in checkerboard.GetValidMoveDestinations(gridPosition))
        {
            grid[position.x, position.y].overlay.SetActive(true);
        }
        
        currentlySelectedPosition = gridPosition;
    }

    private void TurnOffAllOverlays()
    {
        foreach (GridCell cell in grid)
        {
            cell.overlay?.SetActive(false);
        }
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
