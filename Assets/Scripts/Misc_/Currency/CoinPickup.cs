using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int teethValue = 1;
    public float pickupRange = 2f;
    private Transform player;

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        // If player reference is lost (destroyed during evolution), reacquire it
        if (player == null)
        {
            FindPlayer();
            if (player == null) return; // still no player, skip
        }

        if (Vector3.Distance(transform.position, player.position) <= pickupRange)
        {
            CollectCoin();
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void CollectCoin()
    {
        PlayerTeeth.Instance.AddTeeth(teethValue);
        Destroy(gameObject);
    }
}
