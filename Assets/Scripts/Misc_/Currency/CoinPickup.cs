using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
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
        PlayerCurrency.Instance.AddCoins(coinValue);
        Destroy(gameObject);
    }
}
