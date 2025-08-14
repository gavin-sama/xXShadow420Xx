using UnityEngine;

public class FogWall : MonoBehaviour
{
    public float checkDistance = 5f;      // how far to check for a road
    public LayerMask roadLayer;           // set this to the layer your roads are on

    public void TryFall()
    {
        RaycastHit hit;

        // Cast a ray forward to see if there's a road behind this wall
        if (Physics.Raycast(transform.position, transform.forward, out hit, checkDistance, roadLayer))
        {
            // Fall this wall
            FallWall();

            // Try to find a matching wall on the neighbor
            FogWall neighborWall = hit.collider.GetComponentInChildren<FogWall>();
            if (neighborWall != null)
                neighborWall.FallWall();
        }
    }

    public void FallWall()
    {
        if (GetComponent<Rigidbody>() != null) return; // already fallen

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f; // optional: slows down fall
        rb.angularDamping = 0.5f;

        // optional: remove collider so it doesn't block player
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        Destroy(this); // prevent re-triggering
    }
}
