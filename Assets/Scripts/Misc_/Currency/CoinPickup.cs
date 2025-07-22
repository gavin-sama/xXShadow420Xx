using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    public float pickupRange = 2f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= pickupRange)
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        // You can replace this with your own currency system
        PlayerCurrency.Instance.AddCoins(coinValue);
        Destroy(gameObject);
    }
}
