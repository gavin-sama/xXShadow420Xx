using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Change to fit load script ClassMenuController
    public static int SelectedOutfitIndex;

    public float healthPoints = 100;
    public int attackDamage = 100;
    public float attackRange = 100;
    public float attackSpeed = 100;
    public int teeth = 100;
    
    public int totalKills = 100;

    public bool hasResurrection = true;
    public bool extraCoins = true;
    public bool lowHealthStealth = false;

    public int permHealthUpgrades = 0;
    public int permAttackUpgrades = 0;
    public int permSpeedUpgrades = 0;
}