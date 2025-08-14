using UnityEngine;

public class TrackPlayerLight : MonoBehaviour
{
    public string playerTag = "Player";
    private Transform playerTransform;

    void Update()
    {
        // Find player if not already found
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                playerTransform = player.transform;
        }

        // Rotate light to look at player
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform.position);
        }
    }
}