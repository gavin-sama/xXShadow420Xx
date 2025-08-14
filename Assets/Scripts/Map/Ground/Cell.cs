using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public bool filled;
    public List<RoadTypeDirection> tileOptions;
    public int connections = 0;

    public void CreateCell(bool filledState, List<GameObject> roads)
    {
        filled = filledState;
        tileOptions = new List<RoadTypeDirection>();
        foreach (GameObject roadPrefab in roads)
        {
            tileOptions.Add(new RoadTypeDirection { prefab = roadPrefab, rotations = new List<int>() { 0, 90, 180, 270} });
        }
    }

    public void RecreateCell(List<RoadTypeDirection> roadsAndDirection)
    {
        tileOptions = roadsAndDirection;
    }
}
