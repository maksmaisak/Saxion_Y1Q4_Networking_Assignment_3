using UnityEngine;
using UnityEngine.Assertions;

public class MainGamePanel : BasePanel
{
    [SerializeField] BoardView boardView;
    
    public void SetCheckerboard(Checkerboard checkerboard)
    {
        Assert.IsNotNull(boardView);
        boardView.SetCheckerboard(checkerboard);
    }
    
    public void Clear()
    {
        Assert.IsNotNull(boardView);
        boardView.Clear();
    }
}