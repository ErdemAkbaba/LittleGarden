using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;
using File = System.IO.File;

public class DataPersistanceManager : MonoBehaviour
{
    [SerializeField] 
    private string fileName;
    [SerializeField]
    private string oldFileName;

    private GameData gameData;
    private List<IDataPersistance> dataPersistancesObjects;
    private FileDataHandler dataHandler;
    public static DataPersistanceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found More Than One DatapersistanceManager in the Scene");
        }

        instance = this;
    }

    private void Start()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, oldFileName)) != false)
        {
            UIManager.uiManager.notfyPanel.GetComponent<LeanPulse>().RemainingTime = 10;
            UIManager.uiManager.ShowNotfy("New Update! old save files deleted :/",true);

            File.Delete(Path.Combine(Application.persistentDataPath, oldFileName));
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath,fileName);
        this.dataPersistancesObjects = FindAllDataPersistanceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        
        if (gameData==null)
        {
            Debug.Log("No Data Found");
            NewGame();
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistancesObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        } 
    }
    
    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistanceObject in dataPersistancesObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }
        
        dataHandler.SaveData(gameData);
    }

    public void DeleteSave()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistancesObjects =
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistancesObjects);
    }
}
