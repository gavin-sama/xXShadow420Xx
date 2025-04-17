using UnityEngine;

public class CharacterClasses : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class PlayerStats
    {
        public int HP { get; set; }
        public int XP { get; set; }
        public int Coins { get; set; }
        public int Damage { get; set; }

        // Constructor
        public PlayerStats(int hp, int xp, int coins, int damage)
        {
            HP = hp;
            XP = xp;
            Coins = coins;
            Damage = damage;
        }
    }

    public class EnemyStats
    {
        public int HP { get; set; }
        public int Damage { get; set; }

        // Constructor
        public EnemyStats(int hp, int damage)
        {
            HP = hp;
            Damage = damage;
        }
    }


}
