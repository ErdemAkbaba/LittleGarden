using JetBrains.Annotations;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.PlayerAction;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.Windows;
using Random = UnityEngine.Random;
using TMPro;
using System.Runtime.CompilerServices;
using static GridBuildingSystem;

[System.Serializable]
public class TileParts
{
    public Mesh tilePartMesh;
    public string tilePartName;
    public Color explosionColor;
    public Material material;
}

[System.Serializable]
public class BigIslandData
{
    public string name;
    public List<GameObject> tileParts = new List<GameObject>();
    public List<GameObject> islands = new List<GameObject>();
    public int floorIndex;
}

[System.Serializable]
public class Level
{
    public List<Card> unlocedCards = new List<Card>();
    public int requesLevel;
}

[System.Serializable]
public class Rent
{
    public int farmLevel;
    public int rentPrice;
}

[System.Serializable]
public class unlockedQuest
{
    public QuestData questData;
    public int farmLevel;

    public unlockedQuest(QuestData questData, int farmLevel)
    {
        this.questData = questData;
        this.farmLevel = farmLevel;
    }
}

[System.Serializable]
public class RequestExp
{
    public int farmLevel;
    public int requestExp;
}

[System.Serializable]
public class TilePartStatus
{
    public string bigIslandName;
    public List<TileParts> tileParts = new List<TileParts>();
}

public class GameManager : MonoBehaviour, IDataPersistance
{

    public PlayerAction inputActions;

    public List<RequestExp> requestExpPerLevel = new List<RequestExp>();
    FieldChangesTracker changesTracker = new FieldChangesTracker();
    [HideInInspector] public CardUI activeCard;
    [HideInInspector] public UnityEvent mouseClicked;
    [HideInInspector] public UnityEvent nextDay;
    [HideInInspector] public UnityEvent tileChange;
    [HideInInspector] public UnityEvent handCardsChange;
    [HideInInspector] public UnityEvent unlockedIsland;
    [HideInInspector] public UnityEvent farmLevelUp;

    private bool nextDayTransition;
    public bool isActive;
    public GameObject trashPlant;
    public static GameManager gameManager;
    private int currentDay = 1;
    public int gold = 251;
    public GameObject RemovebleObjects;

    [Space(10)]
    [Header("Islands")]
    public List<BigIslandData> bigIslandDatas = new List<BigIslandData>();

    [Space(10)]
    [Header("About Level")]
    public List<Level> levels = new List<Level>();
    public List<unlockedQuest> unlockablequests = new List<unlockedQuest>();
    public int farmLevel;
    public int currentEXP;
    public int requestExp;

    [Space(10)]
    [Header("About Tile")]
    public List<TilePartStatus> tilePartStatus = new List<TilePartStatus>();
    public List<TilePart> spawnedTiles = new List<TilePart>();
    public List<TilePart> activeTiles = new List<TilePart>();
    public TilePart currentTilePart;
    public SpawnClone currentSpawnClone;
    public GameObject explosionFX, waterFX, plantFX, harvestFX;
    public List<Debuff> debuffs = new List<Debuff>();

    [Space(10)]
    [Header("Plants")]
    public List<Plant> plants = new List<Plant>();
    public List<Card> spawnableCards = new List<Card>();
    public List<QuestData> spawnableQuests = new List<QuestData>();

    [Space(10)]
    [Header("About Card Spawn")]
    public GameObject cardCover;
    public List<Card> cards = new List<Card>();
    public List<GameObject> spawnedCards = new List<GameObject>();
    public List<Card> AllCards = new List<Card>();    
    public int maksCardValue;
    [Range(0, 100)] public int toolSpawnRate;
    [Range(0, 100)] public int buildingSpawnRate;
    [Range(0, 100)] public int seedSpawnRate;

    [Space(10)]
    [Header("About Quests")]
    public int maksQuestSize;

    [Space(10)][Header("Debuff")] public GameObject worms;

    [Space(10)][Header("About Rent")] public List<Rent> rents = new List<Rent>();
    public int baseRentDayDistance;
    public int currentRentDayDistance;

    [Space(10)]
    [Header("About Island")]
    public List<Island> islands = new List<Island>();

    private void Awake()
    {
        gameManager = this;
        inputActions = new PlayerAction();
        inputActions.Enable();
        unlockedIsland.AddListener(delegate
        {
            GameManager.gameManager.activeTiles.Clear();
            GameManager.gameManager.activeTiles.AddRange(GameManager.gameManager.spawnedTiles.Where(p => p.gameObject.activeSelf == true));
        });
    }

    void Start()
    {
        for (int i = 0; i < bigIslandDatas.Count; i++)
        {
            for (int a = 0; a < bigIslandDatas[i].islands.Count; a++)
            {
                Island _island = bigIslandDatas[i].islands[a].GetComponent<Island>();                
                bigIslandDatas[i].tileParts.AddRange(_island.islandParts);
            }
        }

        activeTiles.Clear();
        activeTiles.AddRange(spawnedTiles.Where(p => p.gameObject.activeSelf == true));

        UIManager.uiManager.farmLevelSlider.maxValue = requestExp;

        if (mouseClicked == null)
            mouseClicked = new UnityEvent();

        if (handCardsChange == null)
            handCardsChange = new UnityEvent();

        if (nextDay == null)
            nextDay = new UnityEvent();

        if (farmLevelUp == null)
            farmLevelUp = new UnityEvent();

        nextDay.AddListener(DayTransitionTrigger);
        nextDay.AddListener(CalculateRent);
        handCardsChange.AddListener(UIManager.uiManager.TriggerCheckQuest);
        //SpawnCards();

        inputActions.Player.Fire.performed += Clicked();
    }

    private Action<InputAction.CallbackContext> Clicked()
    {
        return (c) =>
        {
            if (currentTilePart != null)
            {
                mouseClicked.Invoke();
            }
        };
    }

    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Tab) && GameObject.FindAnyObjectByType<FloorController>())
        {
            GameObject.FindAnyObjectByType<FloorController>().NextFloor();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && GameObject.FindAnyObjectByType<FloorController>())
        {
            GameObject.FindAnyObjectByType<FloorController>().BackFloor();
        }

        if (inputActions.Player.Q.IsPressed() && FloorController.floorController.isRestaurantOpen == false)
        {
            UIManager.uiManager.hoe.onClick.Invoke();
        }

        if (inputActions.Player.E.IsPressed() && FloorController.floorController.isRestaurantOpen == false)
        {
            UIManager.uiManager.wateringCan.onClick.Invoke();
        }

        if (inputActions.Player.R.IsPressed() && currentSpawnClone == null && FloorController.floorController.isRestaurantOpen == false)
        {
            UIManager.uiManager.trowel.onClick.Invoke();
            //LevelProgress(10);
        }
    }

    private void AddListener(UnityAction action)
    {
        mouseClicked.RemoveAllListeners();
        mouseClicked.AddListener(action);
    }

    public void AddListenerTrowel()
    {
        AddListener(SelectTrowel);
        CursorManager.cursorManager.index = 3;
        AudioManager.audioManager.ChangeAudioClip(2);
    }
    public void AddListenerWateringCan()
    {
        AddListener(SelectWateringCan);
        CursorManager.cursorManager.index = 2;
        AudioManager.audioManager.ChangeAudioClip(2);
    }

    public void AddListenerHoe()
    {
        AddListener(SelectHoe);
        CursorManager.cursorManager.index = 1;
        AudioManager.audioManager.ChangeAudioClip(1);
    }

    public void SelectHoe()
    {
        if (currentTilePart.GetComponent<TilePart>().isPlanted)
            return;

        if (currentTilePart.GetComponent<TilePart>().statusIndex == 1 || currentTilePart.GetComponent<TilePart>().statusIndex == 0)
        {

            Instantiate(explosionFX,
                new Vector3(currentTilePart.transform.position.x, currentTilePart.transform.position.y + 1,
                    currentTilePart.transform.position.z), explosionFX.transform.rotation);
            CameraShake.camerShake.Shake(0.2f, 0.5f);
            currentTilePart.GetComponent<TilePart>().ChangeStatus(2);
            AudioManager.audioManager.TriggerActiveClip();
        }
        else
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouCanPlowJustNonPlowedTiles"));
        }
    }

    public void SelectWateringCan()
    {
        if (currentTilePart.GetComponent<TilePart>().isPlanted)
            return;

        if (currentTilePart.GetComponent<TilePart>().statusIndex == 2)
        {
            Instantiate(waterFX,
                new Vector3(currentTilePart.transform.position.x, currentTilePart.transform.position.y + 1,
                    currentTilePart.transform.position.z), explosionFX.transform.rotation);
            CameraShake.camerShake.Shake(0.2f, 0.5f);
            currentTilePart.GetComponent<TilePart>().ChangeStatus(3);
            AudioManager.audioManager.TriggerActiveClip();
        }

        else
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouCanWaterJustPlowedTiles"));
        }
    }

    public void SelectTrowel()
    {
        TilePart cloneTilePart = currentTilePart.GetComponent<TilePart>();

        if (cloneTilePart.isPlanted && cloneTilePart.childPlant != null && (cloneTilePart.statusIndex != 0 || cloneTilePart.statusIndex != 1))
        {
            Destroy(cloneTilePart.childPlant);
            cloneTilePart.isPlanted = false;
        }

        else
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("NothingToRemove"));
        }

        int newIndex = 0;
        /*if (cloneTilePart.isTilePartDark)
        {
            newIndex = 1;
        }*/

        if(cloneTilePart.isDark)
            cloneTilePart.ChangeStatus(1);
        else
            cloneTilePart.ChangeStatus(0);
        Instantiate(explosionFX, cloneTilePart.plantPos.position, explosionFX.transform.rotation);
    }    

    public void DayTransitionTrigger()
    {
        StartCoroutine(DayTransition());
    }

    public bool Buy(int price)
    {
        if (price <= gold)
        {
            gold -= price;
            UIManager.uiManager.SpawnPopUpStatic(price.ToString(), true);
            foreach (TextMeshProUGUI text in UIManager.uiManager.goldText)
            {
                text.text = gold.ToString();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DayTransition()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            SpawnQuest();
        }

        currentDay++;

        LeanLocalization.SetToken("DAY_COUNT", currentDay.ToString());
        UIManager.uiManager.dayCounter.text = LeanLocalization.GetTranslationText("DayX");

        SpawnCards();
    }

    public void SpawnCards()
    {
        /*spawnedCards.Clear();
        
        for (int i = 0; i < UIManager.uiManager.hand.transform.childCount; i++) 
        {
            Destroy(UIManager.uiManager.hand.transform.GetChild(i).gameObject);
        }*/

        int cardSize = GetCardSize();
        Card.CardTypes cardType = Card.CardTypes.seed;
        for (int i = 0; i < cardSize; i++)
        {
        retry:

            int randomTypeIndex = Random.Range(0, 101);

            if (randomTypeIndex <= seedSpawnRate)
            {
                cardType = Card.CardTypes.seed;
            }
            else if (randomTypeIndex > seedSpawnRate && randomTypeIndex <= toolSpawnRate)
            {
                cardType = Card.CardTypes.tool;
            }

            else if (randomTypeIndex > toolSpawnRate && randomTypeIndex <= buildingSpawnRate)
            {
                cardType = Card.CardTypes.building;
            }

            if (spawnableCards.Where(p => p.currentCardType == cardType).Count() == 0)
            {
                goto retry;
            }

            SpawnCardByIndex(cardType);
        }

        //StartCoroutine(MergeManager.mergeManager.CheckCards());
    }

    public GameObject SpawnCardByIndex(int cardIndex)
    {
        if (spawnedCards.Count >= maksCardValue)
            return null;

        GameObject cloneCard = Instantiate(cardCover, UIManager.uiManager.hand.transform.position,
        cardCover.transform.rotation, UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform);
        cloneCard.GetComponent<CardUI>().card = spawnableCards[cardIndex];
        cloneCard.GetComponent<CardUI>().FillCardInfo();
        spawnedCards.Add(cloneCard);
        //StartCoroutine(MergeManager.mergeManager.CheckCards());
        handCardsChange.Invoke();
        return cloneCard;
    }
    public GameObject SpawnCardByIndex(Card.CardTypes type)
    {
        List<Card> spawnableCardType = new List<Card>(spawnableCards.Where(p => p.currentCardType == type));
        if (spawnedCards.Count >= maksCardValue)
            return null;

        GameObject cloneCard = Instantiate(cardCover, UIManager.uiManager.hand.transform.position,
            cardCover.transform.rotation, UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform);
        cloneCard.GetComponent<CardUI>().card = spawnableCardType[Random.Range(0, spawnableCardType.Count)];
        cloneCard.GetComponent<CardUI>().FillCardInfo();
        spawnedCards.Add(cloneCard);
        //StartCoroutine(MergeManager.mergeManager.CheckCards());
        handCardsChange.Invoke();
        return cloneCard;
    }

    public GameObject SpawnCardByCard(Card card, bool ignore)
    {
        if (spawnedCards.Count >= maksCardValue && !ignore)
            return null;

        GameObject cloneCard = Instantiate(cardCover, UIManager.uiManager.hand.transform.position,
            cardCover.transform.rotation, UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform);
        cloneCard.GetComponent<CardUI>().card = card;
        cloneCard.GetComponent<CardUI>().FillCardInfo();
        spawnedCards.Add(cloneCard);
        //StartCoroutine(MergeManager.mergeManager.CheckCards());
        handCardsChange.Invoke();
        return cloneCard;
    }

    public GameObject SpawnCardByID(string cardID)
    {
        Card cloneCard = AllCards.Find(p => p.cardID == cardID);
        GameObject cloneCardCover = Instantiate(cardCover, UIManager.uiManager.hand.transform.position,
            cardCover.transform.rotation, UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform);
        cloneCardCover.GetComponent<CardUI>().card = cloneCard;
        cloneCardCover.GetComponent<CardUI>().FillCardInfo();
        spawnedCards.Add(cloneCardCover);
        //StartCoroutine(MergeManager.mergeManager.CheckCards());
        handCardsChange.Invoke();
        return cloneCardCover;
    }

    private int GetCardSize()
    {
        int cardSize = 2;
        int cardIndex = currentDay / 5;
        return cardSize + cardIndex;
    }


    private IEnumerator Harvest()
    {
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            if (spawnedTiles[i].gameObject.activeSelf == true)
            {
                spawnedTiles[i].GetComponent<TilePart>().HarvestAnimation();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void CallHarvest()
    {
        StartCoroutine(Harvest());
    }

    public void InvokeNextDay()
    {
        if (nextDayTransition == false)
        {
            StartCoroutine(InvokeNextDayDelay());
        }
    }

    public IEnumerator InvokeNextDayDelay()
    {
        nextDayTransition = true;
        UIManager.uiManager.TriggerTransition();
        yield return new WaitForSeconds(0.7f);
        nextDayTransition = false;
        nextDay.Invoke();
    }

    public void SpawnQuest()
    {
        if (UIManager.uiManager.questParent.transform.childCount < maksQuestSize)
        {
            List<QuestData> spawnedQuestDatas = new List<QuestData>();
            List<QuestData> spawnableQuestDatas = new List<QuestData>();
            //int randomQuestData = Random.Range(0, spawnableQuests.Count);           

            for (int i = 0; i < UIManager.uiManager.allActiveQuest.Count; i++)
            {
                spawnedQuestDatas.Add(UIManager.uiManager.allActiveQuest[i].GetComponent<Quest>().questData);
            }

            spawnableQuestDatas.AddRange(spawnableQuests.Where(p => !spawnedQuestDatas.Contains(p)));
            if (spawnableQuestDatas.Count == 0)
                return;

            int randomQuestData = Random.Range(0, spawnableQuestDatas.Count);
            UIManager.uiManager.spawnQuest(spawnableQuestDatas[randomQuestData]);
        }
    }

    public void LevelProgress(int exp)
    {
        currentEXP += exp;
        if (currentEXP >= requestExp && farmLevel < 11)
        {
            currentEXP = 0;
            requestExp = requestExpPerLevel.Find(p => p.farmLevel == farmLevel + 1).requestExp;
            Level newLevel = levels.Find(p => p.requesLevel == farmLevel + 1);
            //UIManager.uiManager.FillAndShowLevelUpPanel(newLevel.unlocedCards);

            /*for (int i = 0; i < newLevel.unlocedCards.Count; i++)
            {
                if (newLevel.unlocedCards[i].currentCardType != Card.CardTypes.meal)
                {
                    spawnableCards.AddRange(newLevel.unlocedCards);
                }
                else
                {
                    RecipeManager.recipeManager.UnlockNewRecipe(newLevel.unlocedCards[i].recipe);
                }
            }*/

            List<unlockedQuest> unlockedQuestDatas = unlockablequests.Where(p => p.farmLevel == farmLevel + 1).ToList();

            /*if (unlockedQuestDatas.Count > 0)
            {
                for (int i = 0; i < unlockedQuestDatas.Count; i++)
                {
                    spawnableQuests.Add(unlockedQuestDatas[i].questData);
                }
            }*/
            UIManager.uiManager.shopPanel.SetActive(false);
            farmLevel++;
            farmLevelUp.Invoke();
        }
        UIManager.uiManager.farmLevelSlider.value = currentEXP;
        UIManager.uiManager.farmLevelSlider.maxValue = requestExp;
        UIManager.uiManager.currentFarmLevelText.text = farmLevel.ToString();
        UIManager.uiManager.nextFarmLevelText.text = (farmLevel + 1).ToString();
        UIManager.uiManager.farmLevelSliderText.text = currentEXP + "/"
                                                                  + requestExp;
    }

    public void ChangeGold(int value, bool isNegative)
    {
        if (isNegative)
            value = -value;

        gold += value;
        foreach (TextMeshProUGUI text in UIManager.uiManager.goldText)
        {
            text.text = gold.ToString();
        }
    }

    private void CalculateRent()
    {
        currentRentDayDistance--;
        if (currentRentDayDistance <= 0)
        {
            Rent targetRent = rents.Find(p => p.farmLevel == farmLevel);
            if (targetRent != null)
            {
                UIManager.uiManager.RentPanel.OnTransitions.Begin();
                UIManager.uiManager.RentPanelText.text = targetRent.rentPrice.ToString() + "g";
                GameManager.gameManager.isActive = false;
                currentRentDayDistance = baseRentDayDistance;
            }
        }
    }

    public void LoadData(GameData data)
    {
        farmLevel = data.farmLevel;
        currentEXP = data.currentXP;
        requestExp = data.requestXP;
        gold = data.gold;
        foreach (TextMeshProUGUI text in UIManager.uiManager.goldText)
        {
            text.text = data.gold.ToString();
        }

        int farmlevelNext = farmLevel + 1;
        UIManager.uiManager.currentFarmLevelText.text = farmLevel.ToString();
        UIManager.uiManager.nextFarmLevelText.text = farmlevelNext.ToString();
        UIManager.uiManager.farmLevelSlider.value = currentEXP;
        UIManager.uiManager.farmLevelSliderText.text = currentEXP + "/" + requestExp;
        UIManager.uiManager.farmLevelSlider.maxValue = requestExp;        

        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            TilePartData newData = data.TilePartDatas.Find(p => p.name == spawnedTiles[i].name);
            if (newData != null)
            {
                spawnedTiles[i].ChangeStatus(newData.currentTileStatus);                
                if (spawnedTiles[i].statusIndex == 0 || spawnedTiles[i].statusIndex == 1)
                {
                    spawnedTiles[i].GetComponent<TilePart>().ChangeStatus(0); //set light

                    if (((spawnedTiles[i].coordinate.y % 2 == 0) && (spawnedTiles[i].coordinate.x % 2 != 0) || (spawnedTiles[i].coordinate.x % 2 == 0) && (spawnedTiles[i].coordinate.y % 2 != 0)))
                        spawnedTiles[i].GetComponent<TilePart>().ChangeStatus(1); //set dark
                }
                else
                {
                    if (((spawnedTiles[i].coordinate.y % 2 == 0) && (spawnedTiles[i].coordinate.x % 2 != 0) || (spawnedTiles[i].coordinate.x % 2 == 0) && (spawnedTiles[i].coordinate.y % 2 != 0)))
                        spawnedTiles[i].GetComponent<TilePart>().isDark = true;
                }
            }
        }

        List<int> recipeIDs = data.spawnedRecipeID.Where(p => !RecipeManager.recipeManager.spawnableRecipes.Any(q => q.recipeId == p)).ToList();
        List<Recipe> newRecipes = RecipeManager.recipeManager.allRecipes.Where(p => recipeIDs.Contains(p.recipeId)).ToList();
        RecipeManager.recipeManager.spawnableRecipes.AddRange(newRecipes);


        spawnableCards.AddRange(data.spawnableCardData.Where(p => !spawnableCards.Contains(p)));

        List<int> questIds = data.spawnableQuestID.Where(p => !spawnableQuests.Any(q => q.questID == p)).ToList();
        List<unlockedQuest> newQuests = unlockablequests.Where(p => data.spawnableQuestID.Contains(p.questData.questID)).ToList();
        
        for (int i = 0;i < newQuests.Count;i++)
        {
            spawnableQuests.Add(newQuests[i].questData);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.gold = gold;
        data.farmLevel = farmLevel;
        data.currentXP = currentEXP;
        data.requestXP = requestExp;

        data.TilePartDatas.Clear();
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            TilePartData newData = new TilePartData();
            newData.name = spawnedTiles[i].name;
            newData.currentTileStatus = spawnedTiles[i].statusIndex;
            data.TilePartDatas.Add(newData);
        }
        
        data.spawnableCardData.Clear();
        data.spawnableCardData.AddRange(spawnableCards);

        for (int i = 0; i < RecipeManager.recipeManager.spawnableRecipes.Count; i++)
        {
            data.spawnedRecipeID.Add(RecipeManager.recipeManager.spawnableRecipes[i].recipeId);
        }

        for (int i = 0; i < spawnableQuests.Count; i++)
        {
            if (!data.spawnableQuestID.Contains(spawnableQuests[i].questID))
            {
                data.spawnableQuestID.Add(spawnableQuests[i].questID);
            }
        }
    }

    public void GameOver()
    {
        DataPersistanceManager.instance.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeTimeScale(int i)
    {
        Time.timeScale = i;
    }
}