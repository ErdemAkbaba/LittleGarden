using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Lean.Gui;
using Lean.Localization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int sublingsIndex;

    public bool connotSale;
    private List<TilePart> oldTargetTiles = new List<TilePart>();
    
    [HideInInspector] public Transform grid;
    
    public Card.CardTypes cardType;
    public Card card;

    [Header("Card Type Colors And Icons")] public Sprite seedIcon, toolIcon, buildingIcon, animalIcon, mealIcon, plantIcon;
    
    public Color seedColor, toolColor, buildingColor, animalColor, mealColor, plantColor;
    private Vector2 lastMousePosition;
    private Vector3 mouseDelta;
    
    private bool isDragging;
    public bool isGhost;
    public bool isMouseHover;

    private GameObject spawnPreview;
    public GameObject sellParticleFX;
    public GameObject cardCover;
    public GameObject spawnClone;
    public GameObject restaurantSpawnClone;
    private GameObject cardArea;

    private float hoverTime;
    private float defaultYAxis;
    public int targetTileStatus;
    [HideInInspector] public int exp;
    
    [Space(10)] 
    [Header("About Level")]
    public int level = 1;
    public List<GameObject> stars = new List<GameObject>();

    [Space(10)] 
    [Header("About Card UI")] 
    public Image cardTypeIcon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI turnText;
    public GameObject turnIcon;
    public Image icon, shadow;
    public int cardValue;
    public int cardPrice;
    public GameObject unlockLevelPanel;
    public TextMeshProUGUI unlockLevelText;

    public bool startPriceSeted = false;

    [Space(10)]
    [Header("Restaurant")]
    public bool isRestaurantCard;


    public RectTransform border;
    private Vector3 borderOpen;
    private Vector3 borderDef;

    private void Start()
    {

        borderOpen = border.localScale * 1.7f;
        borderDef = border.localScale;

        //Prices
        if (cardType != Card.CardTypes.meal)
        {
            if(level == 1 && !startPriceSeted)
            {
                cardValue = card.cardValue;
                cardPrice = card.price;
                startPriceSeted = true;
            }            
        }        

        if (cardType == Card.CardTypes.seed)
        {
            //PriceDetected();
        }

        lastMousePosition = GameManager.gameManager.inputActions.Player.MousePos.ReadValue<Vector2>();
        //goldText.text = cardValue.ToString() + "g";
        cardCover = transform.GetChild(0).GameObject();
        defaultYAxis = GetComponent<RectTransform>().localPosition.y;
        grid = UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform;
        
        if (!isGhost)
        {
            transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).OnComplete(delegate
            {
                transform.DOScale(new Vector3(1, 1, 1), 0.8f);
            });
        }
        
        GameManager.gameManager.tileChange.AddListener(SelectTargetTile);
        GameManager.gameManager.tileChange.AddListener(CheckBuffRate);

        LeanLocalization.OnLocalizationChanged += FillCardInfo;
    }

    private void OnEnable()
    {
        FillCardInfo();
    }

    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= FillCardInfo;
    }

    int buffRate = 0;
    private void Update()
    {
        if (isMouseHover && hoverTime<0.5f)
        {
            hoverTime += Time.deltaTime;
        }
        
        if (isDragging && GameManager.gameManager.isActive && !card.isNotPlantableCard)
        {
            mouseDelta = GameManager.gameManager.inputActions.Player.MousePos.ReadValue<Vector2>() - lastMousePosition;
            lastMousePosition = GameManager.gameManager.inputActions.Player.MousePos.ReadValue<Vector2>();

            if (cardType== Card.CardTypes.animal)
            {
                List<Building> spawnedBuilding = new List<Building>(GameObject.FindObjectsOfType<Building>());
                
                for (int i = 0; i < spawnedBuilding.Count; i++)
                {
                    if (spawnedBuilding[i].requestCardID == card.cardID)
                    {
                        spawnedBuilding[i].gameObject.transform.parent.GetComponent<TilePart>().SelectEffect();
                    }
                }
            }
            
            if (GameManager.gameManager.currentTilePart != null)
            {
                cardCover.SetActive(false);
                if (spawnPreview == null)
                {
                    if(isRestaurantCard)
                    {
                        GridBuildingSystem.Instance.placedObjectTypeSO = card.placedObjectTypeSO;
                        GridBuildingSystem.Instance.placedObjectCard = card;
                        GridBuildingSystem.Instance.RefreshSelectedObjectType();
                        spawnPreview = new GameObject();
                        /*spawnPreview = Instantiate(restaurantSpawnClone, GameManager.gameManager.currentTilePart.plantPos.position,
                        spawnClone.transform.rotation);*/
                    }
                    else
                    {
                        spawnPreview = Instantiate(spawnClone, GameManager.gameManager.currentTilePart.plantPos.position,
                        spawnClone.transform.rotation);

                        spawnPreview.GetComponent<SpawnClone>().mesh = card.spawnPreviewMesh;
                        spawnPreview.GetComponent<SpawnClone>().FillMesh();
                        spawnPreview.GetComponent<SpawnClone>().targetTileStatus = card.requestTileStatus;
                        spawnPreview.GetComponent<SpawnClone>().costText.text = "-"+ cardPrice + "g";
                    }
                }
            }
            else
            {
                Destroy(spawnPreview);
                GridBuildingSystem.Instance.DeselectObjectType();
                GridBuildingSystem.Instance.placedObjectCard = null;
                cardCover.SetActive(true);
            }
        }
        
        else if(GameManager.gameManager.inputActions.Player.Fire2.IsPressed() && isMouseHover == true && hoverTime>0.2f && !connotSale && card.isRestaurantCard)
        {
            transform.SetParent(grid.parent, true);
            transform.SetSiblingIndex(sublingsIndex);
            GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -45), 0.5f);
            transform.DOScale(Vector3.zero, 0.5f);
            /*GetComponent<RectTransform>().DOMove(UIManager.uiManager.goldText.GetComponent<RectTransform>().position,0.5f).OnComplete(
                delegate
                {
                    UIManager.uiManager.SpawnPopUpStatic(cardValue.ToString(), false);
                    GameManager.gameManager.ChangeGold(cardValue ,false);
                    Vector3 targetPosition = transform.position;
                    targetPosition.z = (transform.position - Camera.main.transform.position).z;
                    Vector3 targetPos = Camera.main.ScreenToWorldPoint(targetPosition);
                    Instantiate(sellParticleFX,targetPos,sellParticleFX.transform.rotation);
                });*/
            Destroy(this.gameObject); 
        }
    }

    public void MoveTo(Vector3 direction)
    {
        transform.SetParent(grid.parent, true);
        transform.SetSiblingIndex(sublingsIndex);
        GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -45), 0.5f);
        transform.DOScale(Vector3.zero, 0.5f);
        GetComponent<RectTransform>().DOMove(direction,0.5f).OnComplete(delegate
        {
            Destroy(gameObject);
        });
    }
    
    public void StartDrag()
    {
        GameManager.gameManager.activeCard = this;
        sublingsIndex = transform.GetSiblingIndex();
        Debug.Log(sublingsIndex);
        isDragging = true;
        GameManager.gameManager.mouseClicked.RemoveAllListeners();
        CursorManager.cursorManager.index = 0;
    }

    public void ReturnHand()
    {
        transform.SetParent(null);
        transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        transform.SetParent(grid);
        transform.SetSiblingIndex(sublingsIndex);
        isDragging = false;
    }
    
    public void EndDrag()
    {
        GameManager.gameManager.activeCard = null;

        if (!GameManager.gameManager.isActive)
        {
            if (cardArea!=null)
            {
                if(cardArea.transform.childCount!=0 && cardArea.transform.GetChild(0).GameObject().tag == "GhostCard")
                {
                    if (cardArea.transform.GetChild(0).GetComponent<CardUI>().card.cardID !=card.cardID 
                        || cardArea.transform.GetChild(0).GetComponent<CardUI>().level !=level)
                    {
                        RecipeManager.recipeManager.ClearAllCards(true);
                    }
                    Destroy(cardArea.transform.GetChild(0).GameObject());
                    transform.SetParent(cardArea.transform);
                    transform.localPosition = Vector3.zero;
                    cardArea.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                }
                
                else if (cardArea.transform.childCount==0)
                {
                    transform.SetParent(cardArea.transform);
                    transform.localPosition = Vector3.zero;
                    cardArea.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                }

                else
                {
                    transform.SetParent(null);
                    transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                    transform.SetParent(grid);
                    transform.SetSiblingIndex(sublingsIndex);
                }
            }
            else
            {
                transform.SetParent(null);
                transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                transform.SetParent(grid);
                transform.SetSiblingIndex(sublingsIndex);
            }

            isDragging = false;
            return;
        }
        
        transform.DORotate(Vector3.zero,0.4f);
        
        /*if (GameManager.gameManager.gold < cardPrice)
        {
            cardCover.SetActive(true);
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughGold"));
            if (spawnPreview!=null)
            {
                Destroy(spawnPreview.gameObject);
            }
        }*/
        
        if (GameManager.gameManager.currentTilePart != null && GameManager.gameManager.currentTilePart.statusIndex == targetTileStatus 
                                                                 && !GameManager.gameManager.currentTilePart.isPlanted && 
                                                                 card.currentCardType != Card.CardTypes.plant && card.currentCardType != Card.CardTypes.animal
                                                                 && card.currentCardType != Card.CardTypes.restaurant)
        {
            
            if (spawnPreview!=null)
            {
                Destroy(spawnPreview.gameObject);
            }
            
            GameObject cloneObject = Instantiate(card.spawnObject, GameManager.gameManager.currentTilePart.plantPos.position,
                card.spawnObject.transform.rotation,
                GameManager.gameManager.currentTilePart.transform);
            GameManager.gameManager.currentTilePart.isPlanted = true;
            GameManager.gameManager.currentTilePart.tilePlant.Invoke(); 
            GameManager.gameManager.currentTilePart.childPlant = cloneObject;
            AudioManager.audioManager.ChangeAudioClip(0);
            AudioManager.audioManager.TriggerActiveClip();
            Instantiate(GameManager.gameManager.plantFX, GameManager.gameManager.currentTilePart.plantPos.position, GameManager.gameManager.plantFX.transform.rotation);
            ClearOldTiles();
            
            switch (cardType)
            {
                case Card.CardTypes.seed :
                    GameManager.gameManager.plants.Add(cloneObject.GetComponent<Plant>());
                    cloneObject.GetComponent<Plant>().level = level;
                    SpawnBuffs(cloneObject.GetComponent<Plant>());
                    //UIManager.uiManager.SpawnPopUpStatic(cardPrice.ToString(),true);
                    break;
                    
                case Card.CardTypes.tool :
                    Debug.Log("Is it Toool!");
                    break;
            }
                
            Destroy(this.gameObject);

        }
        
        else if (GameManager.gameManager.currentTilePart != null && GameManager.gameManager.currentTilePart.isPlanted && card.currentCardType==Card.CardTypes.animal && GameManager.gameManager.currentTilePart.childPlant.GetComponent<Building>())
        {
            
            if (GameManager.gameManager.currentTilePart.childPlant.GetComponent<Building>().requestCardID == card.cardID)
            {
                GameManager.gameManager.currentTilePart.childPlant.GetComponent<Building>().AddCard();

                List<Building> spawnedBuilding = new List<Building>(GameObject.FindObjectsOfType<Building>());

                for (int i = 0; i < spawnedBuilding.Count; i++)
                {
                    if (spawnedBuilding[i].requestCardID == card.cardID)
                    {
                        spawnedBuilding[i].gameObject.transform.parent.GetComponent<TilePart>().UnselectEffect();
                    }
                }
                if (spawnPreview!=null)
                {
                    Destroy(spawnPreview.gameObject);
                }
                
                Destroy(this.gameObject);
            }    
        }

        else if (GameManager.gameManager.currentTilePart != null && card.placedObjectTypeSO != null && card.isRestaurantCard)
        {

            /*GameObject cloneObject = Instantiate(card.spawnObject, spawnPreview.transform.position,
                spawnPreview.GetComponent<SpawnClone>().meshRenderer.transform.rotation);*/

            GridBuildingSystem.Instance.Build();

            if (GridBuildingSystem.Instance.canBuild)
            {
                if (card.placedObjectTypeSO.visual != null)
                {
                    //Destroy(spawnPreview.gameObject);
                    GridBuildingSystem.Instance.DeselectObjectType();
                }

                Destroy(this.gameObject);
            }
            else
            {
                GridBuildingSystem.Instance.DeselectObjectType();
                cardCover.SetActive(true);
                ClearOldTiles();
                UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("CanNotBuildHere!"));
            }
        }

        else
        {
            cardCover.SetActive(true);
            ClearOldTiles();
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouCanPlantJustWetPlowedTiles"));
            if (spawnPreview!=null)
            {
                Destroy(spawnPreview.gameObject);
            }

            List<Building> spawnedBuilding = new List<Building>(GameObject.FindObjectsOfType<Building>());

            for (int i = 0; i < spawnedBuilding.Count; i++)
            {
                if (spawnedBuilding[i].requestCardID == card.cardID)
                {
                    spawnedBuilding[i].gameObject.transform.parent.GetComponent<TilePart>().UnselectEffect();
                }
            }
        }
        
        isDragging = false;
        transform.SetParent(null);
        
        if (cardArea==null)
        {
            transform.SetParent(grid);
            transform.SetSiblingIndex(sublingsIndex);
        }
        else
        {
            transform.SetParent(cardArea.transform);
            transform.position = Vector3.zero;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHover = true;

        border.DOScale(borderOpen, 0.4f).SetEase(Ease.OutBack);

        if (!isDragging && transform.parent == grid)
        {
            Debug.Log(defaultYAxis);
            GetComponent<RectTransform>().DOLocalMoveY(defaultYAxis + 150, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        border.DOScale(borderDef, 0.4f).SetEase(Ease.OutBack);
        isMouseHover = false;
        hoverTime = 0;
        if (!isDragging && transform.parent == grid)
        {
            Debug.Log(defaultYAxis);
            GetComponent<RectTransform>().DOLocalMoveY(defaultYAxis, 0.2f);
        }
    }

    public void FillCardInfo()
    {
        /// fill variable from card
        if (card==null)
            return;
        
        name.text = LeanLocalization.GetTranslationText(card.name);
        
        LeanLocalization.SetToken("PLANT_NAME", name.text);
        LeanLocalization.SetToken("OBJECT_RAPUTATION_VALUE", (((int)(card.reputationValue * 10)) == 0 ? 1 : ((int)(card.reputationValue * 10))).ToString());
        description.text = LeanLocalization.GetTranslationText(card.description);

        icon.sprite = card.icon;
        shadow.sprite = card.icon;
        cardType = card.currentCardType;
        targetTileStatus = card.requestTileStatus;
        isRestaurantCard = card.isRestaurantCard;
        goldText.text = card.price.ToString();

        if (cardType==Card.CardTypes.seed)
        {
            turnText.text = (card.spawnObject.GetComponent<Plant>().phases.Count - 1).ToString();
        }
        else
        {
            turnText.gameObject.SetActive(false);
            //turnIcon.gameObject.SetActive(false);
        }
        //cardCover.GetComponent<Image>().color = card.cardBackColor;

        switch (card.currentCardType)
        {
            case Card.CardTypes.tool:
                cardTypeIcon.sprite = toolIcon;
                cardTypeIcon.color = toolColor;
                break;
            case Card.CardTypes.passiveBuff:
                break;
            case Card.CardTypes.meal:
                cardTypeIcon.sprite = mealIcon;
                cardTypeIcon.color = mealColor;
                break;
            case Card.CardTypes.building:
                cardTypeIcon.sprite = buildingIcon;
                cardTypeIcon.color = buildingColor;
                break;
            case Card.CardTypes.animal:
                cardTypeIcon.sprite = animalIcon;
                cardTypeIcon.color = animalColor;
                break;
            case Card.CardTypes.plant:
                cardTypeIcon.sprite = plantIcon;
                cardTypeIcon.color = plantColor;
                break;
            default:
                break;
        }      
    }

    /*public void LevelUp(int? setLevel = null)
    {
        if (level == 3)
            return;

        if (!startPriceSeted)
        {
            cardValue = card.cardValue;
            cardPrice = card.price;
            startPriceSeted = true;
        }

        if(setLevel == null)
            level += 1;
        else
            level = setLevel.Value;

        for (int i = 0; i < level; i++)
        {
            stars[i].SetActive(true);
        }
        
        if(cardType == Card.CardTypes.seed)
        {
            PriceDetected();
        }
        else if(cardType == Card.CardTypes.plant)
        {
            cardValue = (((cardValue * 3) * (100 + card.priceRate)) / 100);
        }

        goldText.text = cardValue + "g";
    }*/

    /*private void PriceDetected()
    {
        float plantValue = card.spawnObject.GetComponent<Plant>().parentCard.cardValue;
        if (level > 1)
        {
            for (int i = 2; i <= level; i++)
            {
                plantValue = (((plantValue * 3) * (100 + card.spawnObject.GetComponent<Plant>().parentCard.priceRate)) / 100); // level ile çarpılmıyor
            }            
        }        
        cardValue = Mathf.CeilToInt(plantValue * 0.3f);
        cardPrice = Mathf.CeilToInt(plantValue * 0.1f);


        goldText.text = cardValue.ToString() + "g";
    }*/

    public void SpawnBuffs(Plant spawnedPlant)
    {
        for (int i = 0; i < card.ownedBuffs.Count; i++)
        {
            for (int j = 0; j < card.targetTiles.Count; j++)
            {
                Vector2 targetTileCoordinate =
                    GameManager.gameManager.currentTilePart.coordinate + card.targetTiles[j].coordinate;
                GameObject targetTile = FindTargetTile(targetTileCoordinate);
                if (targetTile!=null)
                {
                    spawnedPlant.spawnedBuffs.Add(CopyComponent(card.ownedBuffs[i], targetTile.gameObject));              
                }
            }
        }
    }

    public GameObject FindTargetTile(Vector2 tileCoordinate)
    {
        if (GameObject.Find(tileCoordinate.ToString()))
        {
            TilePart targetTile = GameObject.Find(tileCoordinate.ToString()).GetComponent<TilePart>();
            return targetTile.gameObject;
        }
        else return null;
    }

    public void SelectTargetTile()
    {
        if (isDragging)
        {
            ClearOldTiles();
            
            if (GameManager.gameManager.currentTilePart != null)
            {
                for (int i = 0; i < card.targetTiles.Count; i++) // bu hiç optimize bir şey değil!
                {
                    GameObject targetTile = FindTargetTile(GameManager.gameManager.currentTilePart.GetComponent<TilePart>().coordinate
                                                           + card.targetTiles[i].coordinate);
                    
                    if (targetTile != null)
                    {
                        targetTile.GetComponent<TilePart>().SelectEffect();
                        targetTile.GetComponent<TilePart>().outline.OutlineColor = Color.cyan;
                        targetTile.GetComponent<TilePart>().outline.OutlineMode = Outline.Mode.OutlineAll;
                        oldTargetTiles.Add(targetTile.GetComponent<TilePart>());
                    }
                }   
            }
        }
    }

    private void ClearOldTiles()
    {
        for (int i = 0; i < oldTargetTiles.Count; i++)
        {
            oldTargetTiles[i].UnselectEffect();
            oldTargetTiles[i].outline.OutlineMode = Outline.Mode.OutlineVisible;
            oldTargetTiles[i].outline.OutlineColor = Color.white;
        }

        oldTargetTiles.Clear();
    }
    
    private void CheckBuffRate()
    {
        if (GameManager.gameManager.currentTilePart != null && spawnPreview != null && cardType == Card.CardTypes.seed)
        {
            List<TileBuff> buffs = new List<TileBuff>();
            buffs.Clear();
       
            buffs.AddRange(GameManager.gameManager.currentTilePart.GetComponents<TileBuff>());
        
            buffRate = 0;

            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].targetPlantID == card.spawnObject.GetComponent<Plant>().plantID)
                {
                    buffRate += buffs[i].buffRate;
                }
            }
            Debug.Log("BuffRate :" + buffRate);
            spawnPreview.GetComponent<SpawnClone>().buffText.text = "+" + buffRate.ToString();  
            
        }
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "CardArea"&& transform.parent!=other.transform)
        {
            other.transform.DOScale(new Vector3(1.15f,1.15f,1.15f), 0.5f);
            cardArea = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "CardArea")
        {
            other.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            cardArea = null;
        }
    }

    private void OnDestroy()
    {
        GameManager.gameManager.spawnedCards.Remove(this.GameObject());
        GameManager.gameManager.handCardsChange.Invoke();
    }
}
