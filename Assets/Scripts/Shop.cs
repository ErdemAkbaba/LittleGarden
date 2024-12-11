using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Shop : MonoBehaviour
{
    [HideInInspector] public static Shop shop;

    public ScrollView scrollView;
    public RectTransform scrollViewContent;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ShopData shopData;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

    public Transform tabButtonTransform;
    public Transform shopContent;
    public GameObject tabButtonImage;
    public List<CardUI> cards = new List<CardUI>();
    public List<LeanButton> buttons = new List<LeanButton>();

    public List<Card> spawnableCards = new List<Card>();

    public ShopData ShopData { get => shopData; 
        set 
        {
            shopData = value;
            ChangeShopData(shopData);
        } 
    }

    private void Awake()
    {
        shop = this;
    }

    private void Start()
    {
        CardCreate();

        for (int i = 0; i < buttons.Count; i++)
        {            
            buttons[i].OnClick.AddListener(delegate { ChangeTab(i,false); });
        }

        ChangeTab(0,true);
        scrollViewContent.pivot = new Vector2(0, .5f);
        gameObject.SetActive(false);
    }

    public void CardCreate()
    {
        for (int i = 0; i < shopData.tabs.Count; i++)
        {
            spawnableCards.AddRange(shopData.tabs[i].cards);
            GameObject cloneTabButton = Instantiate(tabButtonImage, Vector3.zero, Quaternion.identity, tabButtonTransform);
            cloneTabButton.name = shopData.tabs[i].buttonName;
            cloneTabButton.transform.Find("Cap").GetChild(0).GetComponent<Image>().sprite = shopData.tabs[i].buttonIcon;
            buttons.Add(cloneTabButton.GetComponent<LeanButton>());          
        }

        spawnableCards = spawnableCards.OrderBy(p => p.unlockLevel).ToList();
        for (int a = 0; a < spawnableCards.Count; a++)
        {
            GameObject cloneCard = Instantiate(GameManager.gameManager.cardCover, shopContent);
            cloneCard.GetComponent<CardUI>().card = spawnableCards[a];
            cloneCard.GetComponent<LeanDrag>().enabled = false;
            cloneCard.GetComponent<CardUI>().FillCardInfo();
            cloneCard.AddComponent<ScrollViewItem>();
            cards.Add(cloneCard.GetComponent<CardUI>());
        }
    }

    public GameObject CardCreate(Card card, Transform parent)
    {
        GameObject cloneCard = Instantiate(GameManager.gameManager.cardCover, parent);
        cloneCard.GetComponent<CardUI>().card = card;
        cloneCard.GetComponent<CardUI>().FillCardInfo();
        return cloneCard;
    }

    public void ChangeTab(int index, bool ignore)
    {
        if(!ignore)
            index = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();

        for (int a = 0; a < buttons.Count; a++)
        {
            buttons[a].gameObject.SetActive(index == shopData.tabs.IndexOf(shopData.tabs.Find(p => p.buttonName == buttons[a].gameObject.name)));
        }

        for (int i = 0; i < cards.Count; i++)
        {            
            if (shopData.tabs[index].cards.Find(p => p == cards[i].card))
            {
                cards[i].gameObject.SetActive(true);
                if (cards[i].card.unlockLevel > GameManager.gameManager.farmLevel)
                {
                    cards[i].GetComponent<LeanDrag>().enabled = false;
                    cards[i].GetComponent<CardUI>().unlockLevelPanel.SetActive(true);
                    cards[i].GetComponent<CardUI>().unlockLevelText.text = cards[i].card.unlockLevel.ToString();
                }
            }                
            else
                cards[i].gameObject.SetActive(false);
        }

        if (shopData.tabs[index].buttonName == "Tool")
        {
            scrollViewContent.pivot = new Vector2(.5f, .5f);
        }
        else
        {
            scrollViewContent.pivot = new Vector2(0, .5f);
        }
    }

    public void UnlockLevelCardCreate(Card card)
    {
    }

    public void ChangeShopData(ShopData newShopData)
    {
        shopData = newShopData;

        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i].gameObject);
        }
        buttons.Clear();

        for (int a = 0; a < cards.Count; a++)
        {
            Destroy(cards[a].gameObject);
        }

        cards.Clear();
        spawnableCards.Clear();

        CardCreate(); for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].OnClick.AddListener(delegate { ChangeTab(i, false); });
        }
        ChangeTab(0, true);
    }
}
    