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

    public virtual List<GameObject> HousePrefabs { get; set; }
    public virtual List<GameObject> GrassPrefabs { get; set; }
    public virtual List<GameObject> StonePrefabs { get; set; }
    public virtual List<GameObject> TreePrefabs { get; set; }


    public abstract float RailLikelihood { get; set; }

    public const float heightAdjustment = 2.418f;
    public static int currentHeightChange = 0;
    public static Vector3 lastTransformPosition = new Vector3(0, 0, 0);

    public int sizeX { get { return 45; } }
    public int sizeZ { get { return 45; } }


    public Vector3 GetNextPosition(Direction direction)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        switch (direction)
        {
            case Direction.North:
                x = lastTransformPosition.x;
                y = (currentHeightChange * heightAdjustment);
                z = lastTransformPosition.z + this.sizeZ;
                break;
            case Direction.East:
                x = lastTransformPosition.x + this.sizeX;
                y = (currentHeightChange * heightAdjustment);
                z = lastTransformPosition.z;
                break;
            case Direction.South:
                x = lastTransformPosition.x;
                y = (currentHeightChange * heightAdjustment);
                z = lastTransformPosition.z + (-1 * this.sizeZ);
                break;
            case Direction.West:
                x = lastTransformPosition.x + (-1 * this.sizeX);
                y = (currentHeightChange * heightAdjustment);
                z = lastTransformPosition.z;
                break;
        }

            return new Vector3(x, y, z);
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