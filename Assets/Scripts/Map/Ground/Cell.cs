using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public bool filled;
    public List<RoadTypeDirection> tileOptions;

    public void CreateCell(bool filledState, List<GameObject> roads)
    {
        filled = filledState;
        tileOptions = new List<RoadTypeDirection>();
        List<Direction> allDirections = new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West};
        foreach (GameObject roadPrefab in roads)
        {
            tileOptions.Add(new RoadTypeDirection { prefab = roadPrefab, rotations = allDirections });
        }
    }

    public void RecreateCell(List<RoadTypeDirection> roadsAndDirection)
    {
        tileOptions = roadsAndDirection;
    }
}
