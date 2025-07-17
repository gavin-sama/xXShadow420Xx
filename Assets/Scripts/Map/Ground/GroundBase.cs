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


    public virtual List<GameObject> HousePrefabs { get; set; }
    public virtual List<GameObject> GrassPrefabs { get; set; }
    public virtual List<GameObject> StonePrefabs { get; set; }
    public virtual List<GameObject> TreePrefabs { get; set; }


    public abstract float RailLikelihood { get; set; }

    public const double heightAdjustment = 2.418;
    public virtual int sizeX { get { return 45; } }
    public virtual int sizeZ { get { return 45; } }
}

[Serializable]
public class RoadTypeDirection
{
    public GameObject prefab;
    public List<int> rotations;
}