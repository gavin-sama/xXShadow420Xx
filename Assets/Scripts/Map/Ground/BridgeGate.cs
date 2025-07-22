using System.Collections.Generic;
using UnityEngine;

public class BridgeGate : GroundBase
{
    [field: SerializeField] public override List<RoadTypeDirection> NorthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> EastRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> SouthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> WestRoadPrefabs { get; set; }

    public override Direction[] PlaceableDirections { get { return new Direction[] { Direction.North, Direction.South }; } }

    [field: SerializeField] public override float RailLikelihood { get; set; } = 0.35f;
    public override int sizeX { get { return 15; } }
    public override int sizeZ { get { return 15; } }
}
