using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SiloItem
{
    public string floorName;
    public Card plant;
    public int count;
    public GameObject siloUI;
    public SiloItem(Card _plant, int _count, GameObject _siloUI, string _floorName)
    {
        plant = _plant;
        count = _count;
        siloUI = _siloUI;
        floorName = _floorName;
    }    
}

[System.Serializable]
public class FloorCards
{
    public string floorName;
    public List<Card> allPlants = new List<Card>();
}

public class Silo : MonoBehaviour, IDataPersistance
{
    public UnityEvent stockChange;
    public static Silo silo;    
    public List<SiloItem> stock = new List<SiloItem>();
    public List<GameObject> stockUIs = new List<GameObject>();
    public Transform siloUIParent;
    public GameObject siloUI;

    public List<FloorCards> FloorPlants = new List<FloorCards>();

    private void Awake()
    {
        silo = this;

        if (stockChange == null)
            stockChange = GetComponent<UnityEvent>();
    }
    private void OnEnable()
    {
        StartCoroutine(Silo.silo.FloorChanged());
    }

    private void Start()
    {
        for (int i = 0; i < FloorPlants.Count; i++)
        {
            for (int a = 0; a < FloorPlants[i].allPlants.Count; a++)
            {
                GameObject clone = Instantiate(siloUI, Vector3.zero, Quaternion.identity, siloUIParent);
                clone.GetComponent<Animator>().SetBool("Active", true);
                SiloItem item = new SiloItem(FloorPlants[i].allPlants[a], 0, clone, FloorPlants[i].floorName);

                if(itemDatas.Find(p => p.plant == FloorPlants[i].allPlants[a]) != null)
                {
                    item.count = itemDatas.Find(p => p.plant == FloorPlants[i].allPlants[a]).count;
                }                

                stock.Add(item);
                clone.transform.GetChild(1).GetComponent<Image>().sprite = FloorPlants[i].allPlants[a].icon;

                clone.transform.GetComponentInChildren<TextMeshProUGUI>().text = item.count.ToString();
                if (FloorPlants[i].floorName != FloorController.floorController.floorItems[FloorController.floorController.currentFloorIndex].name)
                    clone.SetActive(false);

                stockUIs.Add(clone);

                Debug.Log(clone.GetComponent<Animator>().GetBool("Active"));
            }
        }

        Debug.LogError(stock.Count);
    }

    public IEnumerator FloorChanged()
    {
        List<GameObject> siloUIWillOpen = new List<GameObject>();        
       
       
        for (int i = 0; i < stock.Count; i++)
        {
            if (stock[i].floorName != FloorController.floorController.floorItems[FloorController.floorController.currentFloorIndex].name)
            {
                stock[i].siloUI.gameObject.GetComponent<SiloUI>().Close();
            }
            else
            {
                siloUIWillOpen.Add(stock[i].siloUI.gameObject);
            }
        }
               

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < siloUIWillOpen.Count; i++)
        {
            siloUIWillOpen[i].GetComponent<SiloUI>().Open();
        }
    }

    public void CreateSiloItem(Card parentCard, Plant plant)
    {       
        SiloItem siloItem = stock.Find(p => p.plant == parentCard);
        if (siloItem != null)
        {            
            siloItem.count++;
            siloItem.siloUI.transform.GetComponentInChildren<TextMeshProUGUI>().text = siloItem.count.ToString();
        }

        Transform target = stock.Find(p => p.plant == parentCard).siloUI.transform;

        if (target.position != Vector3.zero)
            plant.SpawnQuestSprite(target);
        else
            plant.SpawnQuestSprite(siloUIParent);

        stockChange.Invoke();        
    }

    public void RemoveSiloStock(int count, Card card)
    {
        SiloItem siloItem = stock.Find(p => p.plant == card);
        siloItem.count -= count;
        siloItem.siloUI.transform.GetComponentInChildren<TextMeshProUGUI>().text = siloItem.count.ToString();

        /*if (siloItem.count <= 0)
        {
            stock.Remove(siloItem);
        }*/

        stockChange.Invoke();
    }

    public void AddSiloStock(int count, Card card)
    {
        SiloItem siloItem = stock.Find(p => p.plant == card);
        siloItem.count += count;
        siloItem.siloUI.transform.GetComponentInChildren<TextMeshProUGUI>().text = siloItem.count.ToString();

        stockChange.Invoke();
    }

    public List<SiloItemData> itemDatas = new List<SiloItemData>();

    /*public void LoadData(GameData data)
    {
        for (int i = 0; i < data.SiloItems.Count; i++)
        {
            itemDatas.Add(data.SiloItems[i]);
        }
    }

    public void SaveData(ref GameData data)
    {        
        for (int i = 0; i < 10; i++)
        {
            SiloItemData siloItemData = new SiloItemData();

            siloItemData.floorName = stock[i].floorName;
            siloItemData.count = stock[i].count;
            siloItemData.plant = stock[i].plant;

            data.SiloItems.Add(siloItemData);
        }
    }*/

    public void LoadData(GameData data)
    {
        for (int i = 0; i < data.SiloDataItems.Count; i++)
        {
            itemDatas.Add(data.SiloDataItems[i]);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.SiloDataItems.Clear();

        for (int i = 0; i < stock.Count; i++)
        {
            SiloItemData siloItemData = new SiloItemData();

            siloItemData.floorName = stock[i].floorName;
            siloItemData.count = stock[i].count;
            siloItemData.plant = stock[i].plant;

            data.SiloDataItems.Add(siloItemData);
        }
    }
}
