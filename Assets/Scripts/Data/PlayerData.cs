using UnityEngine;

[System.Serializable]
public class PlayerData
{
    
    public static int SelectedOutfitIndex;

    public float healthPoints = 100;
    public int attackDamage = 100;
    public float attackRange = 100;
    public float attackSpeed = 100;
    public int teeth = 100;
    
    public int killsLastRun = 0;
    public int totalKills = 100;

    public bool hasResurrection = false;
    public bool extraCoins = false;
    public bool lowHealthStealth = false;

    public int permHealthUpgrades = 0;
    public int permAttackUpgrades = 0;
    public int permSpeedUpgrades = 0;
}