using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Lean.Localization.LeanPhrase;

public class QuestGenerator : MonoBehaviour
{
    public List<Card> cards = new List<Card>();

    [SerializeField]
    public List<QuestData> questDatas = new List<QuestData>();

    public int questSize = 10;
    
    public void Generate()
    {
        //cards = cards.Where(p => p.currentCardType == Card.CardTypes.plant).ToList();
        questDatas.Clear();
        GameManager.gameManager.unlockablequests.Clear();
        for (int i = 0; i < questSize; i++)
        {
            int questLevel = 0;
            int randomCard = Random.Range(0, cards.Count);

            if (cards.Count == 0)
                break;

            Card card = cards[randomCard];

            

            Level level = GameManager.gameManager.levels.Find(
                p => p.unlocedCards.Any(
                    x => x.cardID == card.cardID ||
                    (
                        x.spawnObject != null && 
                        x.spawnObject.GetComponent<Plant>() != null && 
                        x.spawnObject.GetComponent<Plant>().parentCard.cardID == card.cardID
                    )
                )
            );



            if (level == null)
            {
                Debug.LogWarning($"Card undecided -> {card.name}");
                continue;                
            }          

            questLevel = level.requesLevel;

            int randomLevel = 1;//Random.Range(1, 4);
            int randomPlantCount = 1; //Random.Range(1, 6);

            QuestData questData = new QuestData();            
            questData.requestPlantIcon = card.icon;
            questData.requestPlantName = card.name;

            questData.Name = $"{randomPlantCount} {card.name} (Level {randomLevel})";

            /*LeanLocalization.SetToken("RANDOM_PLANT_COUNT", randomPlantCount.ToString());
            LeanLocalization.SetToken("CARD_NAME", card.name);
            LeanLocalization.SetToken("RANDOM_LEVEL", randomLevel.ToString());
            questData.Description = LeanLocalization.GetTranslationText("QuestDesc");*/

            questData.rewardExp = Mathf.CeilToInt(CardLevelExpCalc(card , randomLevel) * randomPlantCount);
            questData.rewardGold = Mathf.CeilToInt(CardLevelPriceCalc(card, randomLevel) * randomPlantCount);
            questData.questID = Random.Range(111111, 999999) + i;
            questData.requestPlantLevel = randomLevel;
            questData.requestPlantCount = randomPlantCount;
            questData.requestCard_ID = card.cardID;
#if UNITY_EDITOR

            AssetDatabase.CreateAsset(questData, "Assets/Quests/" + questData.Name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            GameManager.gameManager.unlockablequests.Add(new unlockedQuest(questData, questLevel));
            questDatas.Add(questData);
            cards.Remove(card);// bu kalkacak
        }
    }

    public static float CardLevelExpCalc(Card card, int level)
    {
        float plantValue = 0;
        if (card.currentCardType == Card.CardTypes.plant)
        {
            plantValue = card.spawnObject.GetComponent<Plant>().rewardExpValues[level-1];
        }else if(card.currentCardType == Card.CardTypes.meal)
        {
            float recipeExp = 0;
            /*foreach (Card recipeCard in card.recipe.requestCards)
            {
                recipeExp += CardLevelExpCalc(recipeCard, level);
            }*/
            plantValue = recipeExp;
        }
        
        plantValue = Mathf.CeilToInt(plantValue * 1.6f);        
        return plantValue;
    }

    public static float CardLevelPriceCalc(Card card, int level)
    {
        float plantValue = 0f;
        if (card.currentCardType == Card.CardTypes.plant)
        {
            plantValue = card.spawnObject.GetComponent<Plant>().parentCard.cardValue;
            if (level > 1)
            {
                for (int i = 2; i <= level; i++)
                {
                    plantValue = (((plantValue * 3) * (100 + card.spawnObject.GetComponent<Plant>().parentCard.priceRate)) / 100); // level ile çarpýlmýyor
                }
            }
        }else if (card.currentCardType == Card.CardTypes.meal)
        {
            float recipePrice = 0;
            /*foreach (Card recipeCard in card.recipe.requestCards)
            {
                recipePrice += CardLevelPriceCalc(recipeCard, level);
            }*/
            plantValue = recipePrice;
        }

        plantValue = Mathf.CeilToInt(plantValue * 1.6f);
        return plantValue;
    }
}
