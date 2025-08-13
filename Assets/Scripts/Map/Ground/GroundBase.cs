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


    public abstract float RailLikelihood { get; set; }

    public const float heightAdjustment = 2.418f;
    public float height = 0;
    public static Transform lastTransform;

    public int sizeX { get { return 45; } }
    public int sizeZ { get { return 45; } }


    public Vector3 GetNextPosition(Direction direction)
    {
        Vector3 transformFlatForward = new Vector3(this.gameObject.transform.forward.x, 0, this.gameObject.transform.forward.z);
        Vector3 transformFlatRight = new Vector3(this.gameObject.transform.right.x, 0, this.gameObject.transform.right.z);
        Vector3 nextPosition;


        if (direction == Direction.North)
        {
            nextPosition = this.gameObject.transform.position + transformFlatForward * 45f;
            //Debug.Log($"North Transform Flat: {transformFlatForward} -- Transform Change: {transformFlatForward * 45}");
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else if (direction == Direction.East)
        {
            nextPosition = this.gameObject.transform.position + transformFlatRight * 45f;
            //Debug.Log($"East Transform Flat: {transformFlatRight} -- Transform Change: {transformFlatRight * 45f}");
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else if (direction == Direction.South)
        {
            nextPosition = this.gameObject.transform.position + (-transformFlatForward) * 45f;
            //Debug.Log($"South Transform Flat: {-transformFlatForward} -- Transform Change: {(-transformFlatForward) * 45}");
            nextPosition.y = currentHeightChange * heightAdjustment;
        }
        else
        {
            nextPosition = this.gameObject.transform.position + (-transformFlatRight) * 45f;
            //Debug.Log($"West Transform Flat: {-transformFlatRight} -- Transform Change: {(-transformFlatRight) * 45f}");
            nextPosition.y = currentHeightChange * heightAdjustment;
        }


        return new Vector3(
            Mathf.Round(nextPosition.x / 45f) * 45f,
            nextPosition.y,
            Mathf.Round(nextPosition.z / 45f) * 45f
        );
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

    public List<int> originalRotations;
    public bool? rotationsChanged;

    public RoadTypeDirection Clone()
    {
        return new RoadTypeDirection
        {
            prefab = this.prefab,
            rotations = new List<int>(this.rotations),
            originalRotations = this.rotations,
            rotationsChanged = false
        };
    }
}

public enum Direction
{
    North, East, South, West
}