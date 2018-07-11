using UnityEngine;
using UnityEngine.Assertions;

/// Plays a checkers game on a boardview offline.
public class TestBoardPlayer : MonoBehaviour
{
    [SerializeField] BoardView boardView;

    private Checkerboard checkerboard;

    void Start()
    {
        Assert.IsNotNull(boardView);

        checkerboard = CheckersHelper.MakeDefaultCheckerboard();
        boardView.SetCheckerboard(checkerboard);
        boardView.SetControlsEnabled(true);
        boardView.OnMoveRequest += OnMoveRequest;
        //checkerboard.AddAt(new Vector2Int(3, 3), Checkerboard.TileState.Black);
    }

    private void OnMoveRequest(BoardView sender, Vector2Int origin, Vector2Int target)
    {
        Debug.Log($"OnMoveRequest: from {origin} to {target}");
        checkerboard.TryMakeMove(origin, target);
    }
}
