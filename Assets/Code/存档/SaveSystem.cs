using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");

    public static void SaveGame(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Save failed: save data is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.saveId))
        {
            data.saveId = GenerateSaveId();
        }

        if (!Directory.Exists(SaveFolder))
        {
            Directory.CreateDirectory(SaveFolder);
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(SaveFolder, data.saveId + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Save succeeded: " + path);
    }

    public static SaveData LoadGame(string saveId)
    {
        if (string.IsNullOrWhiteSpace(saveId))
        {
            Debug.LogError("Load failed: save id is empty.");
            return null;
        }

        string path = Path.Combine(SaveFolder, saveId + ".json");

        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static List<SaveData> GetAllSaves()
    {
        List<SaveData> result = new List<SaveData>();

        if (!Directory.Exists(SaveFolder))
        {
            return result;
        }

        string[] files = Directory.GetFiles(SaveFolder, "*.json");

        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data != null && !string.IsNullOrWhiteSpace(data.saveId))
                {
                    result.Add(data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Failed to read save file: " + file + "\n" + ex.Message);
            }
        }

        result.Sort((a, b) => string.CompareOrdinal(b.saveId, a.saveId));
        return result;
    }

    public static bool HasAnySave()
    {
        return GetAllSaves().Count > 0;
    }

    public static SaveData GetLatestSave()
    {
        List<SaveData> saves = GetAllSaves();
        return saves.Count > 0 ? saves[0] : null;
    }

    public static string GenerateSaveId()
    {
        return System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
    }
}
