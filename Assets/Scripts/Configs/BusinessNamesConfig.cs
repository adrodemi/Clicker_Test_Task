using UnityEngine;

[CreateAssetMenu(fileName = "BusinessNamesConfig", menuName = "Configs/Business Names")]
public class BusinessNamesConfig : ScriptableObject
{
    public BusinessNameEntry[] entries;
}

[System.Serializable]
public struct BusinessNameEntry
{
    public int businessId;
    public string businessName;
    public string upgrade1Name;
    public string upgrade2Name;
}