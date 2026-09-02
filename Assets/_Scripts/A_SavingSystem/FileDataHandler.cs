using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        string backupPath = fullPath + ".bak";
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            loadedData = LoadDataFromFile(fullPath);
        }

        if (loadedData == null && File.Exists(backupPath))
        {
            Debug.LogWarning($"Primary save file corrupted or unreadable. Attempting backup recovery: {backupPath}");
            loadedData = LoadDataFromFile(backupPath);
        }

        return loadedData;
    }

    private GameData LoadDataFromFile(string path)
    {
        try
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(dataToLoad))
            {
                Debug.LogWarning($"Save file at {path} was completely empty.");
                return null;
            }

            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading file at path: {path}\n{e}");
            return null;
        }
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        string tempPath = fullPath + ".tmp";
        string backupPath = fullPath + ".bak";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            if (File.Exists(fullPath))
            {
                File.Copy(fullPath, backupPath, true);
            }

            if (File.Exists(tempPath))
            {
                File.Copy(tempPath, fullPath, true);
                File.Delete(tempPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to save data to file: {fullPath}\n{e}");
        }
    }
}