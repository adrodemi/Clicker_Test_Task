using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public float balance;
    public List<BusinessSave> businesses = new();
}

[Serializable]
public class BusinessSave
{
    public int id;
    public int level;
    public float progress;
    public bool upgrade1Bought;
    public bool upgrade2Bought;
}