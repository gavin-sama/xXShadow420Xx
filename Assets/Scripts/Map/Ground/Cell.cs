using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool filled;
    public Tile[] tileOptions;

    public void CreateCell(bool filledState, Tile[] tiles)
    {
        filled = filledState;
        tileOptions = tiles;
    }

    public void RecreateCell(Tile[] tiles)
    {
        tileOptions = tiles;
    }
}
