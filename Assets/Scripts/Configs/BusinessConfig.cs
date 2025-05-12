using UnityEngine;

[CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/Business")]
public class BusinessConfig : ScriptableObject
{
    public int businessId;
    public float incomeDelay;
    public float baseCost;
    public float baseIncome;

    public UpgradeData upgrade1;
    public UpgradeData upgrade2;
}