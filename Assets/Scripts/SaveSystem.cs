using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // pretty print
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found at: " + SavePath);
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}
