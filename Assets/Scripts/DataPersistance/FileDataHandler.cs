using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler : MonoBehaviour
{ 
    public string dataDirPath = "";
    public string dataFileName = "";
    public string oldFileName = "";
    public static FileDataHandler instance;
    
    private void Awake()
    {
        instance = this;
    }

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {        
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullpath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullpath,FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Datayı yüklerken sorunla karşılaşıldı : " + fullpath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void SaveData(GameData data)
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream fileStream = new FileStream(fullpath,FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Datayı kaydederken sorunla karşılaşıldı : " + fullpath + "\n" + e);
        }
    }
}
