using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public abstract class GroundBase : MonoBehaviour
{
    public abstract List<RoadTypeDirection> NorthRoadPrefabs { get; set; }
    public abstract List<RoadTypeDirection> EastRoadPrefabs { get; set; }
    public abstract List<RoadTypeDirection> SouthRoadPrefabs { get; set; }
    public abstract List<RoadTypeDirection> WestRoadPrefabs { get; set; }

    public abstract Direction[] PlaceableDirections { get; }
    public abstract int extraEnds { get; }

    public virtual List<GameObject> HousePrefabs { get; set; }
    public virtual List<GameObject> GrassPrefabs { get; set; }
    public virtual List<GameObject> StonePrefabs { get; set; }
    public virtual List<GameObject> TreePrefabs { get; set; }


    public abstract float RailLikelihood { get; set; }

    public const float heightAdjustment = 2.418f;
    public static int currentHeightChange = 0;
    public static Transform lastTransform;

    public int sizeX { get { return 45; } }
    public int sizeZ { get { return 45; } }


    public Vector3 GetNextPosition(Direction direction)
    {
        Vector3 lastTransformFlat = new Vector3(lastTransform.forward.x, 0, lastTransform.forward.z).normalized;
        Vector3 nextPosition;

        if (direction == Direction.North)
        {
            nextPosition = lastTransform.position + lastTransformFlat * 45f;
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else if (direction == Direction.East)
        {
            nextPosition = lastTransform.position + (Quaternion.Euler(0, 90, 0) * lastTransformFlat) * 45f;
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else if (direction == Direction.South)
        {
            nextPosition = lastTransform.position + (-lastTransformFlat) * 45f;
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else
        {
            nextPosition = lastTransform.position + (Quaternion.Euler(0, -90, 0) * lastTransformFlat) * 45f;
            nextPosition.y = currentHeightChange * heightAdjustment;
        }

        return nextPosition;
    }

    public List<Vector3> GetAvailablePositions()
    {
        List<Vector3> availablePositions = new List<Vector3>();

        foreach (Direction direction in PlaceableDirections)
        {
            availablePositions.Add(GetNextPosition(direction));
        }

        return availablePositions;
    }
}

[Serializable]
public class RoadTypeDirection
{
    public GameObject prefab;
    public List<int> rotations;
}

public enum Direction
{
    North, East, South, West
}