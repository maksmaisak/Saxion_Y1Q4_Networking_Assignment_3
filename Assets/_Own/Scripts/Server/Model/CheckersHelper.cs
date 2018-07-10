using UnityEngine;

public static class CheckersHelper 
{
    public static Checkerboard MakeDefaultCheckerboard()
    {
        var checkerboard = new Checkerboard();

        const int NumFilledRows = 3;

        for (int y = 0; y < NumFilledRows; ++y)
        {
            for (int x = 0; x < checkerboard.size.x; ++x)
            {
                if ((x % 2 == 0) ^ (y % 2 == 0)) continue;
                checkerboard.AddAt(new Vector2Int(x, y), Checkerboard.TileState.White);
            }
        }
        
        for (int y = checkerboard.size.y - NumFilledRows; y < checkerboard.size.y; ++y)
        {
            for (int x = 0; x < checkerboard.size.x; ++x)
            {
                if ((x % 2 == 0) ^ (y % 2 == 0)) continue;
                checkerboard.AddAt(new Vector2Int(x, y), Checkerboard.TileState.Black);
            }
        }

        return checkerboard;
    }
}