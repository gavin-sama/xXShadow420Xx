using System.Collections.Generic;
using UnityEngine;

public class RoadRamp : GroundBase
{
    [field: SerializeField] public override List<RoadTypeDirection> NorthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> EastRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> SouthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> WestRoadPrefabs { get; set; }


    [field: SerializeField] public override List<GameObject> GrassPrefabs { get; set; }
    [field: SerializeField] public override List<GameObject> StonePrefabs { get; set; }
    [field: SerializeField] public override List<GameObject> TreePrefabs { get; set; }

    [field: SerializeField] public override float RailLikelihood { get; set; } = 0.65f;
    public override int sizeZ { get { return 15; } }
}
