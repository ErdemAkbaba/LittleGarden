using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUIParent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool isDragging;
    public bool isMouseHover;
    private float defaultYAxis;
    [HideInInspector] public GameObject cloneObject;
    public GameObject cardCover;
    [HideInInspector] public Transform grid;
    public GameObject spawnClone;
    public Card card;
    
    [Space(10)] 
    [Header("About Level")]
    public int level = 1;
    public List<GameObject> stars = new List<GameObject>();
    
    [Space(10)] 
    [Header("About Card UI")] 
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public Image icon, shadow;
    
    public virtual void Start()
    {
        cardCover = transform.GetChild(1).gameObject;
        defaultYAxis = GetComponent<RectTransform>().localPosition.y;
        grid = transform.parent;
        
        //start animation
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).OnComplete(delegate
        {
            transform.DOScale(new Vector3(1, 1, 1), 0.8f);
        });
    }
    
    public void FillCardInfo()
    {
        /// fill variable from card
        name.text = card.name;
        description.text = card.description;
        icon.sprite = card.icon;
        shadow.sprite = card.icon;
        //cardCover.GetComponent<Image>().color = card.cardBackColor;
        ////
    }


    void Update()
    {
        if (isDragging)
        {
            if (GameManager.gameManager.currentTilePart != null)
            {
                cardCover.SetActive(false);
                if (cloneObject == null)
                {
                    cloneObject = Instantiate(spawnClone, GameManager.gameManager.currentTilePart.plantPos.position,
                        spawnClone.transform.rotation);

                    //cloneObject.GetComponent<SpawnClone>().mesh = card.plant.GetComponent<Plant>().meshFilter.sharedMesh;
                    //cloneObject.GetComponent<SpawnClone>().FillMesh();
                }                

            }
            else
            {
                Destroy(cloneObject);
                cardCover.SetActive(true);
            }
        }
        else if(Input.GetMouseButtonDown(1) && isMouseHover == true)
        {
            //SellCard();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) //animation buglu
    {
        isMouseHover = true;
        if (!isDragging)
        {
            GetComponent<RectTransform>().DOLocalMoveY(defaultYAxis + 150, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHover = false;
        if (!isDragging)
        {
            GetComponent<RectTransform>().DOLocalMoveY(defaultYAxis, 0.2f);
        }
    }
    
    public void SellCard()
    {
        //transform.DOScale(Vector3.zero, 0.5f);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(delegate
        {
            GameManager.gameManager.gold += card.cardValue;
            Destroy(this.gameObject);
        });
    }
    
    public void StartDrag()
    {
        isDragging = true;
        GameManager.gameManager.mouseClicked.RemoveAllListeners();
        CursorManager.cursorManager.index = 0;
    }

    public void EndDrag()
    {
        isDragging = false;
        
        /*if (GameManager.gameManager.gold < card.price)
        {
            cardCover.SetActive(true);
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughGold"));
            if (cloneObject!=null)
            {
                Destroy(cloneObject.gameObject);
            }
        }*/
        
        /*else
        {
        }*/
            PlaceCard();
        
        transform.SetParent(null);
        transform.SetParent(grid);
    }

    public virtual void PlaceCard()
    {
        
    }
    
    /// <summary>
    /// Base her zaman eklenmeli!
    /// </summary>
    public virtual void LevelUp()
    {
        level +=1;
        for (int i = 0; i < level; i++)
        {
            stars[i].SetActive(true);
        }
    }
}

