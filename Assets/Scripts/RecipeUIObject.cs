using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using Lean.Localization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUIObject : MonoBehaviour
{
    public TextMeshProUGUI recipeNameText;
    public Image recipeImage;
    public Recipe recipe;
    public int exp;

    private string recipeName;
    private void OnEnable()
    {
        recipeNameText.text = LeanLocalization.GetTranslationText(recipeName);
    }

    public void FillData(string recipeName, Sprite recipeIcon, Recipe recipeData,int expData)
    {
        recipeNameText.text = LeanLocalization.GetTranslationText(recipeName);
        this.recipeName = recipeName;
        recipeImage.sprite = recipeIcon;
        recipe = recipeData;
        exp = expData;
    }

    public void CallCards()
    {
        RecipeManager.recipeManager.ClearAllCards(false);

        for (int i = 0; i < recipe.requestCards.Count; i++)
        {
            GameObject cloneCard = Instantiate(RecipeManager.recipeManager.ghostCard, Vector3.zero,
                RecipeManager.recipeManager.ghostCard.transform.rotation,RecipeManager.recipeManager.cardAreas[i].transform);
            //cloneCard.GetComponent<CardUI>().card = recipe.requestCards[i];
            cloneCard.GetComponent<CardUI>().FillCardInfo();
            cloneCard.transform.localPosition = Vector3.zero;
        }
    }
}
