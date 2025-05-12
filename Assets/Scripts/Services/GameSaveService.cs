using UnityEngine;

public static class GameSaveService
{
    private static readonly string SaveKey = "Clicker_Save_v1";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return null;

        string json = PlayerPrefs.GetString(SaveKey);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}