using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public static int SelectedOutfitIndex;

    public float healthPoints;
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public int teeth;

    public int killsLastRun = 0;
    public int totalKills = 0;
}