using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class FogWall : MonoBehaviour
{
    public float checkDistance = 0.1f;      // how far to check for a road

    public void TryFall()
    {
        Vector3 origin = this.transform.position;
        GroundBase groundScript = (GroundBase)this.transform.parent.gameObject.GetComponentAtIndex(1);
        
        foreach (Direction direction in groundScript.PlaceableDirections)
        {
            Vector3 neighborPos = origin + groundScript.GetNextPosition(direction);

            GameObject neighbor = RoadGenerator.gridComponents.FirstOrDefault(obj => Vector3.Distance(obj.transform.position, neighborPos) < checkDistance);
            if (neighbor != null)
            {
                neighbor.transform.GetComponentInChildren<FogWall>().FallWall();
                Transform child = neighbor.transform.Find("Fog");
                List<FogWall> walls = new List<FogWall>();
                if (child != null)
                    child = child.Find("Collapsable");
                    if (child.name != null)
                        for (int i = 0; i < child.childCount; i++)
                            walls.Add(child.GetChild(i).GetComponent<FogWall>());

                float minDist = float.MaxValue;

                foreach (FogWall wall in walls)
                {
                    float dist = Vector3.Distance(origin, wall.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        wall.FallWall();
                    }
                }
            }
        }
    }

    public void FallWall()
    {
        if (gameObject.activeSelf == false)
            return; // already fallen

        bool thing = GetComponent<ParticleSystem>().main.loop;
        thing = false;

        // optional: remove collider so it doesn't block player
        BoxCollider col = GetComponent<BoxCollider>();
        col.enabled = false;

        Destroy(this); // prevent re-triggering
    }
}
