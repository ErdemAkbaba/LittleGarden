using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "RedrubyFarm/Quest")]
public class QuestData : ScriptableObject
{
    public int questID;
    public Sprite requestPlantIcon;
    public string requestPlantName;
    public string Name;
    [TextArea]
    public string Description;
    public int rewardExp;
    public int rewardGold;
    public int requestPlantLevel;
    public int requestPlantCount;
    public string requestCard_ID;
}
