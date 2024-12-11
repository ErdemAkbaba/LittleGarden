using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardData
{
    public string cardID;
    public int level;
    public string handName;
}

[System.Serializable]
public class TilePartData
{
   public string name;
   public int currentTileStatus;
}

[System.Serializable]
public class SpawnedPlant
{
   public string parentName;
   public string cardID;
   public int CurrentDayCount;
   public int level;
   public bool isGrowth;
}

[System.Serializable]
public class SpawnedBuilding
{
   public string parentName;
   public string cardID;
   public int currentAnimalCount;
}

[System.Serializable]
public class DebuffObjects
{
   public string parentName;
   public string debuffName;
}

[System.Serializable]
public class SpawnedTool
{
   public string parentName;
   public string cardID;
}

[System.Serializable]
public class SiloItemData
{
    public string floorName;
    public Card plant;
    public int count;
}

[System.Serializable]
public class MealCountData
{
    public Recipe recipe;
    public int count;
}

[System.Serializable]
public class GridCellData
{
    public Vector2 cellCoordinate;
    public Vector3 cellWorldPosition;
    public bool isObjectPlaced;
    public Island parentIsland;
    public string objectCard;
    public Vector3 objectEulerAngles;
    public PlacedObjectTypeSO.Dir dir;
}

public class GameData
{
   public List<CardData> handCards = new List<CardData>();
   public List<TilePartData> TilePartDatas = new List<TilePartData>();
    public List<MealCountData> MealCountData = new List<MealCountData>();
    public List<GridCellData> GridCellDatas = new List<GridCellData>();
   public List<SpawnedPlant> SpawnedPlants = new List<SpawnedPlant>();
   public List<SpawnedBuilding> SpawnedBuildings = new List<SpawnedBuilding>();
   public List<SiloItemData> SiloDataItems = new List<SiloItemData>();
   public List<SpawnedTool> SpawnedTools = new List<SpawnedTool>();
   //public List<Island> unlockedIsland = new List<Island>();
   public List<int> islandIDs = new List<int>();
   public List<Card> spawnableCardData = new List<Card>();
   public List<DebuffObjects> spawnedDebuff = new List<DebuffObjects>();
   public List<int> spawnableQuestID = new List<int>();   
   public List<int> spawnedRecipeID = new List<int>();
   public bool tutorialShowed = false;

   [Header("GameManager")]
   public int farmLevel = 1;
   public int currentXP = 0;
   public int requestXP = 100;
   public int gold = 100;
   
   public GameData()
   {
      CardData cardData = new CardData();
        cardData.cardID = "T_1";
        cardData.handName = "Restaurant";

        CardData cardData2 = new CardData();
        cardData2.cardID = "C_1";
        cardData2.handName = "Restaurant";

        handCards.Add(cardData);
        handCards.Add(cardData2);
    }
}
