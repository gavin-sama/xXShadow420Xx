using System.Collections.Generic;
using UnityEngine;

public class RoadCorner : GroundBase
{
    [field: SerializeField] public override List<RoadTypeDirection> NorthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> EastRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> SouthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> WestRoadPrefabs { get; set; }

    public override Direction[] PlaceableDirections { get { return new Direction[] { Direction.North, Direction.West }; } }

    public override int extraEnds { get { return 0; } }

    [field: SerializeField] public override float RailLikelihood { get; set; } = 0.35f;
}
