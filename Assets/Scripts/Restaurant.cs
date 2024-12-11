using Lean.Gui;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Meal
{
    public Recipe recipe;
    public int count;

    public Meal(Recipe _recipe, int _count)
    {
        recipe = _recipe;
        count = _count;
    }

    public int Count { get => count; 
        set 
        {
            count = value;
        } 
    }
}

[Serializable]
public class Chairs
{
    public GameObject chair;
    public bool isSeatable;

    public Chairs(GameObject _chair, bool _isSeatable)
    {
        chair = _chair;
        isSeatable = _isSeatable;
    }
}

public class Restaurant : MonoBehaviour, IDataPersistance
{
    public static Restaurant restourant;
    public List<Meal> meals = new List<Meal>();
    public List<Chairs> chairs = new List<Chairs>();

    public List<GameObject> furniture = new List<GameObject>();

    public float reputation = 0;
    public GameObject npc;
    public Slider reputationSlider;
    public NavMeshSurface meshSurface;
    public ShopData restaurantShopData;
    public GameObject inGameCanvas;

    public GameObject receiptPanel;
    public UnityEvent mealCountChange;

    public int npcMaxSpawnTime;
    public int npcMinSpawnTime;

    public bool isRestaurantOpen;

    private void Awake()
    {
        restourant = this;
    }

    private void Start()
    {
        List<CellData> cells = new List<CellData>(GridBuildingSystem.Instance.cellData.Where(p => p.isObjectPlaced));
        float totalRepValue = 0;
        foreach (CellData cell in cells)
        {
            totalRepValue += cell.objectCard.reputationValue;
        }

        reputationSlider.value = totalRepValue;

        if (mealCountChange == null)
        {
            mealCountChange = new UnityEvent();
        }

        mealCountChange.AddListener(MealsCountCheck);
    }

    public void MealsCountCheck()
    {
        if (meals.Count == 0)
        {
            List<NPC> npcs = FindObjectsOfType<NPC>().ToList();

            foreach (NPC npc in npcs)
            {
                Destroy(npc.gameObject);
            }

            Receipt.Instance.FillReceipt();
            receiptPanel.GetComponent<LeanWindow>().TurnOn();
            GameManager.gameManager.ChangeTimeScale(0);
        }
    }

    private void Update()
    {
        if (meals.Count == 0)
        {
            CancelInvoke("NpcStart");
        }
    }    

    public void PlayButton()
    {
        if (chairs.Where(p => p.isSeatable == true).Count() > 0 && meals.Count > 0)
        {
            isRestaurantOpen = true;
            //Debug.Log("Table");
            //UIManager.uiManager.timeControlPanel.SetActive(true);
            UIManager.uiManager.openRestourantButton.SetActive(false);
            UIManager.uiManager.shopButton.SetActive(false);
            UIManager.uiManager.shopPanel.SetActive(false);
            meshSurface.BuildNavMesh();
            InvokeRepeating("NpcStart", UnityEngine.Random.Range(2, 4), UnityEngine.Random.Range(npcMinSpawnTime, npcMaxSpawnTime));
        }
        else
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouHaveToPlaceSomething"));
        }       
    }

    public void CloseRestaurant()
    {
        CancelInvoke("NpcStart");

        for (int i = 0; i < furniture.Count; i++)
        {
            furniture[i].SetActive(false);
        }

        List<NPC> npcs = new List<NPC>(GameObject.FindObjectsOfType<NPC>());

        for (int i = 0; i < npcs.Count; i++)
        {
            Destroy(npcs[i].gameObject);
        }

        for (int i = 0; i < chairs.Count; i++)
        {
            chairs[i].isSeatable = true;
        }
    }

    public void CloseFurnitures()
    {
        for (int i = 0; i < furniture.Count; i++)
        {
            furniture[i].SetActive(false);
        }

        List<NPC> npcs = new List<NPC>(GameObject.FindObjectsOfType<NPC>());

        for (int i = 0; i < npcs.Count; i++)
        {
            Destroy(npcs[i].gameObject);
        }

        for (int i = 0; i < chairs.Count; i++)
        {
            chairs[i].isSeatable = true;
        }
    }

    public void NpcStart()
    {
        if (chairs.Where(p => p.isSeatable == true).Count() <= 0)
            return;

        GameObject cloneNpc = Instantiate(npc);
        cloneNpc.transform.localPosition = NPC.npc.RandomSpawnpoint();
    }

    public void AddReputation(float _reputation)
    {
        if (reputationSlider.value <= reputationSlider.maxValue)
        {
            reputationSlider.value += _reputation;
        }
        else
        {
            reputationSlider.value = reputationSlider.maxValue;
        }
    }

    public void ReduceReputation(float _reputation)
    {
        if (reputation > 0)
        {
            reputationSlider.value -= _reputation;
        }
        else
        {
            reputationSlider.value = 0;
        }
    }

    public void RestaurantEditMode()
    {
        if (meals.Count > 0)
        {
            FloorController.floorController.restaurantBackPanel.SetActive(false);
            //UIManager.uiManager.OpenShop();
            inGameCanvas.SetActive(true);
            Shop.shop.ShopData = restaurantShopData;
            CameraController.cameraController.speed = CameraController.cameraController.defSpeed;
            UIManager.uiManager.CloseLeanWindow(UIManager.uiManager.restaurantMenu);
            UIManager.uiManager.CloseLeanWindow(UIManager.uiManager.restaurantCustomizedWindow);
            UIManager.uiManager.openRestourantButton.SetActive(true);
            UIManager.uiManager.shopButton.SetActive(true);
            UIManager.uiManager.restaurantPanel.GetComponent<Image>().enabled = false;
            UIManager.uiManager.floorPanel.SetActive(false);

            for (int i = 0; i < UIManager.uiManager.hands.Count; i++)
            {
                UIManager.uiManager.hands[i].handTransform.gameObject.SetActive(UIManager.uiManager.hands[i].handName == 
                    FloorController.floorController.floorItems[FloorController.floorController.currentFloorIndex].name);

                GameManager.gameManager.handCardsChange.Invoke();
            }
        }
        else
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouHaveToCookFirst"));
        }

        for (int i = 0; i < furniture.Count; i++)
        {
            furniture[i].SetActive(true);
        }
    }

    public List<MealCountData> mealCountDatas = new List<MealCountData>();
    public void LoadData(GameData data)
    {
        for (int i = 0; i < data.MealCountData.Count; i++)
        {
            mealCountDatas.Add(data.MealCountData[i]);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.MealCountData.Clear();

        for (int i = 0; i < meals.Count; i++)
        {
            MealCountData mealData = new MealCountData();

            mealData.count = meals[i].Count;
            mealData.recipe = meals[i].recipe;

            data.MealCountData.Add(mealData);
        }
    }
}
