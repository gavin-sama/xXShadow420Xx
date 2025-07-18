using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadStraight : GroundBase
{
    [field: SerializeField] public override List<RoadTypeDirection> NorthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> EastRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> SouthRoadPrefabs { get; set; }
    [field: SerializeField] public override List<RoadTypeDirection> WestRoadPrefabs { get; set; }


    [field: SerializeField] public override List<GameObject> HousePrefabs { get; set; }
    [field: SerializeField] public override List<GameObject> GrassPrefabs { get; set; }
    [field: SerializeField] public override List<GameObject> StonePrefabs { get; set; }
    [field: SerializeField] public override List<GameObject> TreePrefabs { get; set; }

    [field: SerializeField] public override float RailLikelihood { get; set; } = 0.35f;
    public override int sizeZ { get { return 30; } }

    private int rotation = 0;
    private GameObject recentCreation;

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            if (rotation == 0)
            {
                RoadTypeDirection mapToGenerate = NorthRoadPrefabs[Random.Range(0, NorthRoadPrefabs.Count - 1)];
                GameObject prefabToGenerate = mapToGenerate.prefab;

                if (prefabToGenerate.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
                {
                    GroundBase prefabScript = prefabToGenerate.GetComponentAtIndex(1) as GroundBase;
                    Vector3 newPosition = GetNextPosition(prefabScript.sizeX, prefabScript.sizeZ, Direction.North);
                    Quaternion rotate = Quaternion.identity;
                    rotate.y = mapToGenerate.rotations[Random.Range(0, mapToGenerate.rotations.Count - 1)];

                    recentCreation = Instantiate(prefabToGenerate, newPosition, rotate);
                    rotation++;
                }
            }
            else
            {
                Destroy(recentCreation);
                rotation--;
            }
        }
    }
}
