using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "RedrubyFarm/Card")]
public class Card : ScriptableObject
{
    public enum CardTypes
    {
        plant,
        seed,
        tool,
        passiveBuff,
        meal,
        building,
        restaurant,
        animal
    }
    public string cardID;
    [ConditionalField("currentCardType", true, CardTypes.restaurant)]
    public Mesh spawnPreviewMesh;
    public CardTypes currentCardType;

    [ConditionalField("currentCardType", false, CardTypes.meal)]
    public Recipe recipe;

    [ConditionalField("currentCardType", false, CardTypes.restaurant)]
    public PlacedObjectTypeSO placedObjectTypeSO;
    [ConditionalField("currentCardType", false, CardTypes.restaurant)]
    public float reputationValue;

    public bool isNotPlantableCard;
    public bool isMergable;
    public bool isRestaurantCard;
    public string name;

    [TextArea]
    public string description;

    public int unlockLevel;
    public int requestTileStatus;
    public Sprite icon;
    [ConditionalField("currentCardType", true, CardTypes.restaurant)]
    public GameObject spawnObject;
    public Color cardBackColor;
    public int spawnDay = 0;
    public int cardValue = 10;
    public int price;
    public int priceRate;
    public List<TargetTile> targetTiles = new List<TargetTile>();
    public List<TileBuff> ownedBuffs = new List<TileBuff>();
}
