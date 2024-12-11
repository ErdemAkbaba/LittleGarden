using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager recipeManager;
    public Card trashCard;
    public List<Recipe> spawnableRecipes = new List<Recipe>();
    public List<Recipe> allRecipes = new List<Recipe>();
    public GameObject recipeUIObject;
    public GameObject resultCardArea;
    public List<GameObject> cardAreas = new List<GameObject>();
    public GameObject ghostCard;
    private void Awake()
    {
        recipeManager = this;
    }

    void Start()
    {
        for (int i = 0; i < spawnableRecipes.Count; i++)
        {
            if (!spawnableRecipes[i])
                continue;

            /*RecipeUIObject newRecipeUI = Instantiate(recipeUIObject, Vector3.zero, recipeUIObject.transform.rotation,
                UIManager.uiManager.recipeParent.transform).GetComponent<RecipeUIObject>();

            if(newRecipeUI)            
                newRecipeUI.FillData(spawnableRecipes[i].recipeName, spawnableRecipes[i].recipeIcon, spawnableRecipes[i], spawnableRecipes[i].exp);

            */
        }
    }

    public void CheckRecipe()
    {
        if (resultCardArea.transform.childCount != 0 &&
               resultCardArea.transform.GetChild(0).gameObject.tag != "GhostCard" &&
               resultCardArea.transform.GetChild(0).GetComponent<CardUI>())
        {
            resultCardArea.transform.GetChild(0).gameObject.GetComponent<CardUI>().ReturnHand();
        }

        Recipe targetRecipe = null;
        List<Card> cards = new List<Card>();
        List<GameObject> cardsGameobject = new List<GameObject>();

        for (int i = 0; i < cardAreas.Count; i++)
        {
            if (cardAreas[i].transform.childCount>0)
            {
                if (cardAreas[i].transform.GetChild(0).GameObject().tag!="GhostCard")
                {
                    cards.Add(cardAreas[i].transform.GetChild(0).GetComponent<CardUI>().card);
                    cardsGameobject.Add(cardAreas[i].transform.GetChild(0).gameObject);   
                }
            }
        }

        for (int i = 0; i < spawnableRecipes.Count; i++)
        {
            //bool isMatched = DoListsMatch(cards, spawnableRecipes[i].requestCards);
            /*if (isMatched)
            {
                targetRecipe = spawnableRecipes[i];
                break;
            }*/
        }

        if (targetRecipe != null)
        {           
            GameObject resultCard = GameManager.gameManager.SpawnCardByCard(targetRecipe.resultCardData[0], true);
            resultCard.transform.SetParent(resultCardArea.transform);
            resultCard.transform.localPosition = Vector3.zero;

            CardUI recipeCardUI = resultCard.GetComponent<CardUI>();

            //Recipe Price
            int recipePrice = 0; // start price
            int recipeExp = 0;
            int detectedLevel = 3;
            foreach (GameObject cardObject in cardsGameobject)
            {
                CardUI handCardUI = cardObject.GetComponent<CardUI>();
                if (!handCardUI)
                    continue;

                recipePrice += handCardUI.cardValue;
                recipeExp += handCardUI.card.spawnObject.GetComponent<Plant>().expValue;

                if (handCardUI.level < detectedLevel)
                {
                    detectedLevel = handCardUI.level;
                }
            }
            float rate = (1f + ((float)recipeCardUI.card.priceRate / 100f));            
            recipePrice = Mathf.CeilToInt(recipePrice * rate);
            recipeExp = Mathf.CeilToInt(recipeExp * rate);

            resultCard.GetComponent<CardUI>().exp = recipeExp;
            //Set new price
            recipeCardUI.cardValue = recipePrice;
            recipeCardUI.startPriceSeted = true;
            //Set recipe level
            //recipeCardUI.LevelUp(detectedLevel);
            //Exp Progress
            GameManager.gameManager.LevelProgress(recipeExp);           

        }
        else if(cards.Count != 0)
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("Trash"));
            GameObject resultCard = GameManager.gameManager.SpawnCardByCard(trashCard,true);
            resultCard.transform.SetParent(resultCardArea.transform);
            resultCard.transform.localPosition = Vector3.zero;
        }
        
        for (int i = 0; i < cardsGameobject.Count; i++)
        {
            Destroy(cardsGameobject[i].gameObject);
        }
        cardsGameobject.Clear();

    }

    public IEnumerator CallResultCard(GameObject resultCard)
    {
        yield return new WaitForSeconds(1);
        resultCard.GetComponent<CardUI>().ReturnHand();
    }
    
    private bool DoListsMatch(List<Card> list1, List<Card> list2)
    {
        var areListsEqual = true;
        
        if (list1.Count != list2.Count)
            return false;
        
        list1.Sort(SortByScore);
        list2.Sort(SortByScore);
        
        for (var i = 0; i < list1.Count; i++)
        {
            Debug.Log(list1[i] + " " + list2[i]);
            if (list2[i] != list1[i])
            {
                areListsEqual = false;
            }
        }

        return areListsEqual;
    }
    
    static int SortByScore(Card card1, Card card2)
    {
        return card1.cardID.CompareTo(card2.cardID);
    }

    public void ClearAllCards(bool justGhost)
    {
        for (int i = 0; i < cardAreas.Count; i++)
        {
            if (cardAreas[i].transform.childCount>0)
            {
                if (cardAreas[i].transform.GetChild(0).GameObject().tag=="GhostCard")
                {
                    Destroy(cardAreas[i].transform.GetChild(0).GameObject());
                }
                else if(!justGhost)
                {
                    cardAreas[i].transform.GetChild(0).GetComponent<CardUI>().ReturnHand();
                }
            }
        }

        if (resultCardArea.transform.childCount>0)
        {
            if (resultCardArea.transform.GetChild(0).GameObject().tag=="GhostCard")
            {
                Destroy(resultCardArea.transform.GetChild(0).GameObject());
            }
            else if(!justGhost)
            {
                resultCardArea.transform.GetChild(0).GetComponent<CardUI>().ReturnHand();
            }
        }
    }
    
    public void UnlockNewRecipe(Recipe recipe)
    {
        spawnableRecipes.Add(recipe);
        RecipeUIObject newRecipeUI = Instantiate(recipeUIObject, Vector3.zero, recipeUIObject.transform.rotation,
            UIManager.uiManager.recipeParent.transform).GetComponent<RecipeUIObject>();
        newRecipeUI.FillData(recipe.recipeName,recipe.recipeIcon,recipe,recipe.exp);
        newRecipeUI.transform.SetAsFirstSibling();
    }
}
