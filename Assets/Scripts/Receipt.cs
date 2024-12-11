using Lean.Gui;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ReceiptItem
{
    public Sprite mealIcon;
    public string mealName;
    public int mealCount;
    public int totalValue;
    public Meal meal;

    public ReceiptItem(Sprite _mealIcon, string _mealName, int _mealCount, int _totalValue, Meal _meal) 
    {
        mealIcon = _mealIcon;
        mealName = _mealName;
        mealCount = _mealCount;
        totalValue = _totalValue;
        meal = _meal;
    }
}

public class Receipt : MonoBehaviour
{
    public static Receipt Instance;

    public List<ReceiptItem> receiptItems = new List<ReceiptItem>();
    public GameObject receiptItemPrefab;
    public GameObject receiptHolder;
    public TextMeshProUGUI totalTipText;
    public TextMeshProUGUI totalEarningText;
    public int totalTip;
    public int totalEarning;

    private void Awake()
    {
        Instance = this;
    }

    public void FillReceipt()
    {

        for (int i = 0; i < receiptHolder.transform.childCount; i++)
        {
            Destroy(receiptHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < receiptItems.Count; i++)
        {
            GameObject cloneReceiptItem = Instantiate(receiptItemPrefab, receiptHolder.transform);
            cloneReceiptItem.transform.GetChild(0).GetComponent<Image>().sprite = receiptItems[i].mealIcon;
            cloneReceiptItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = LeanLocalization.GetTranslationText(receiptItems[i].mealName);
            cloneReceiptItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = receiptItems[i].mealCount.ToString();
            cloneReceiptItem.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = receiptItems[i].totalValue.ToString();
        }

        totalTipText.text = totalTip.ToString();
        totalEarningText.text = (totalEarning + totalTip).ToString();
    }

    public void OkayButton()
    {
        Restaurant.restourant.isRestaurantOpen = false;
        GameManager.gameManager.ChangeGold(totalEarning + totalTip, false);
        totalTip = 0;
        totalEarning = 0;
        totalTipText.text = totalTipText.ToString();
        totalEarningText.text = totalEarning.ToString();
        receiptItems.Clear();
        UIManager.uiManager.timeControlPanel.SetActive(false);
        Restaurant.restourant.CloseRestaurant();
        GameManager.gameManager.ChangeTimeScale(1);
        FloorController.floorController.BackFloor(0);
        transform.GetChild(0).GetComponent<LeanWindow>().TurnOff();
    }
}
