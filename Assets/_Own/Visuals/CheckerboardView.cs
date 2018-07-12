using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckerboardView : MonoBehaviour
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

    public delegate void OnMoveRequestHandler(CheckerboardView sender, Vector2Int origin, Vector2Int target);
    public event OnMoveRequestHandler OnMoveRequest;
    
    private Checkerboard checkerboard;
    private GridCell[,] grid;

    private Vector2Int? currentlySelectedPosition;
    private bool controlsEnabled;
    
    private struct GridCell
    {
        public BoardTile tile;
        public ColorOverlay overlay;
        public PieceView pieceView;
    }
    
    public void SetCheckerboard(Checkerboard newCheckerboard)
    {
        Assert.IsNotNull(newCheckerboard);
        if (checkerboard != null && checkerboard != newCheckerboard) Clear();
        
        checkerboard = newCheckerboard;

        InitializeGrid();
        checkerboard.OnPieceAdded   += CheckerboardOnPieceAdded;
        checkerboard.OnPieceMoved   += CheckerboardOnPieceMoved;
        checkerboard.OnPieceRemoved += CheckerboardOnPieceRemoved;
    }
    
    public void SetOwnColor(Checkerboard.TileState tileState)
    {
        Assert.AreNotEqual(Checkerboard.TileState.None, tileState);

        float targetRotation = tileState == Checkerboard.TileState.Black ? 180f : 0f;
        transform.localRotation = Quaternion.Euler(0f, targetRotation, 0f);
        
        // TODO also enable/disable controls based on own color.
    }
    
    public void Clear()
    {
        if (checkerboard != null)
        {
            checkerboard.OnPieceAdded -= CheckerboardOnPieceAdded;
            checkerboard.OnPieceMoved -= CheckerboardOnPieceMoved;
            checkerboard.OnPieceRemoved -= CheckerboardOnPieceRemoved;
            checkerboard = null;
        }

        DestroyAllChildren(tilesParent);
        DestroyAllChildren(overlaysParent);
        DestroyAllChildren(piecesParent);
        grid = null;
    }

    public void SetControlsEnabled(bool newControlsEnabled)
    {
        controlsEnabled = newControlsEnabled;
        if (!newControlsEnabled)
        {
            ClearSelection();
        }
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
        
        grid[x, y].tile    = AddTile(gridPosition);
        grid[x, y].overlay = AddOverlay(gridPosition);
        
        Checkerboard.TileState tile = checkerboard.GetAt(gridPosition);
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
        tile.transform.localPosition = TilespaceToLocalspace(gridPosition);
        tile.OnClick += (sender) => OnClickTileView(sender, gridPosition);
        return tile;
    }

    private ColorOverlay AddOverlay(Vector2Int gridPosition)
    {
        Assert.IsNotNull(overlayPrefab); 
        
        ColorOverlay overlay = Instantiate(overlayPrefab, overlaysParent);
        overlay.transform.localPosition = TilespaceToLocalspace(gridPosition);
        
        return overlay;
    }

    private PieceView AddPiece(Vector2Int gridPosition, bool isWhite)
    {        
        PieceView prefab = isWhite ? whiteCheckerPiecePrefab : blackCheckerPiecePrefab;
        Assert.IsNotNull(prefab);
        
        PieceView piece = Instantiate(prefab, piecesParent);
        piece.transform.localPosition = TilespaceToLocalspace(gridPosition);
        piece.gridPosition = gridPosition;
        piece.OnClick += (sender) => PieceViewOnClick(sender, sender.gridPosition);
        
        return piece;
    }
    
    private void PieceViewOnClick(PieceView sender, Vector2Int gridPosition)
    {
        if (!controlsEnabled) return;
        if (checkerboard.GetAt(gridPosition) != checkerboard.currentPlayer) return;
        
        if (currentlySelectedPosition.HasValue)
        {
            ClearSelection();
        }

        grid[gridPosition.x, gridPosition.y].overlay.SetActive(true);
        currentlySelectedPosition = gridPosition;

        foreach (Vector2Int position in checkerboard.GetValidMoveDestinations(gridPosition))
        {
            grid[position.x, position.y].overlay.SetActive(true);
        }
    }

    private void OnClickTileView(BoardTile sender, Vector2Int gridPosition)
    {
        if (!currentlySelectedPosition.HasValue) return;
        if (currentlySelectedPosition.Value == gridPosition) return;
        if (!checkerboard.IsValidMove(currentlySelectedPosition.Value, gridPosition)) return;

        Vector2Int origin = currentlySelectedPosition.Value;
        Vector2Int target = gridPosition;
        OnMoveRequest?.Invoke(this, origin, target);
        
        ClearSelection();
    }

    private void CheckerboardOnPieceAdded(Checkerboard sender, Vector2Int position)
    {
        ClearSelection();
        
        grid[position.x, position.y].pieceView = AddPiece(position, sender.GetAt(position) == Checkerboard.TileState.White);
    }
   
    private void CheckerboardOnPieceMoved(Checkerboard sender, Vector2Int origin, Vector2Int target)
    {
        ClearSelection();
        
        PieceView pieceView = grid[origin.x, origin.y].pieceView;
        
        pieceView.MoveTo(TilespaceToLocalspace(target));
        pieceView.gridPosition = target;
        
        grid[target.x, target.y].pieceView = pieceView;
        grid[origin.x, origin.y].pieceView = null;
    }
    
    private void CheckerboardOnPieceRemoved(Checkerboard sender, Vector2Int position)
    {
        ClearSelection();
        
        grid[position.x, position.y].pieceView.Capture();
        grid[position.x, position.y].pieceView = null;
    }
    
    private void ClearSelection()
    {
        foreach (GridCell cell in grid)
        {
            cell.overlay?.SetActive(false);
        }
        
        currentlySelectedPosition = null;
    }
    
    private Vector3 TilespaceToLocalspace(Vector2Int gridPosition)
    {
        Vector2 tileSize = new Vector2(1f, 1f);
        
        Vector2 position = Vector2.Scale(tileSize, gridPosition);
        position -= Vector2.Scale(tileSize, checkerboard.size) * 0.5f;
        position += tileSize * 0.5f;

        return new Vector3(position.x, 0f, position.y);
    }

    private static void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent.Cast<Transform>().ToArray()) Destroy(child.gameObject);
    }
}
