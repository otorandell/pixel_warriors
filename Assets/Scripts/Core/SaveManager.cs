using System.IO;
using UnityEngine;

namespace PixelWarriors
{
    public static class SaveManager
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static void Save(RunData runData)
        {
            SaveData saveData = SaveData.FromRunData(runData);
            string json = JsonUtility.ToJson(saveData, false);
            File.WriteAllText(SavePath, json);
        }

        public static RunData Load()
        {
            if (!HasSave()) return null;

            try
            {
                string json = File.ReadAllText(SavePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                return saveData.ToRunData();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SaveManager] Failed to load save: {e.Message}");
                return null;
            }
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}
