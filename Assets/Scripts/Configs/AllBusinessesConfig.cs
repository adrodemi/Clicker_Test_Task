using UnityEngine;

[CreateAssetMenu(fileName = "AllBusinessesConfig", menuName = "Configs/All Businesses")]
public class AllBusinessesConfig : ScriptableObject
{
    public BusinessConfig[] businesses;
}