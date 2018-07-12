using UnityEngine;
using UnityEngine.Assertions;

/// Plays a checkers game on a boardview offline.
public class TestBoardPlayer : MonoBehaviour
{
    [SerializeField] CheckerboardView checkerboardView;

    private Checkerboard checkerboard;

    void Start()
    {
        Assert.IsNotNull(checkerboardView);

        checkerboard = CheckersHelper.MakeDefaultCheckerboard();
        checkerboardView.SetCheckerboard(checkerboard);
        checkerboardView.SetControlsEnabled(true);
        checkerboardView.OnMoveRequest += OnMoveRequest;
        //checkerboard.AddAt(new Vector2Int(3, 3), Checkerboard.TileState.Black);
    }

    private void OnMoveRequest(CheckerboardView sender, Vector2Int origin, Vector2Int target)
    {
        Debug.Log($"OnMoveRequest: from {origin} to {target}");
        checkerboard.TryMakeMove(origin, target);
        
        checkerboardView.SetOwnColor(checkerboard.currentPlayerColor);
    }
}
