using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Localization;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TilePart : MonoBehaviour, IDataPersistance
{
    public UnityEvent tileHarvest;
    public UnityEvent tilePlant;
    public UnityEvent tileStatusCahnged;
    
    public Vector2 coordinate;
    [HideInInspector] public Transform plantPos;
    [HideInInspector] public float defY, UpperY;
    [HideInInspector] public int statusIndex;
    public bool isPlanted;
    //public bool isTilePartDark;
    [HideInInspector] public GameObject childPlant;
    
    private float overTrigger = 1f, overClone;
    public Outline outline;
    public MeshFilter meshFilter;
    public GameObject uiPanel;
    public Sprite upgrateIcon, waterIcon, tilePartIcon, plantGrowth;
    public Image iconImage;

    public bool isDark;
    public bool isRestaurantTile;
    private bool hover;

    public List<GridPointCube> gridPoints = new List<GridPointCube>();
    public GameObject gridPoint;
    private float gridSize = 0.5f;

    public Vector3 defScale;

    private void Awake()
    {
        gameObject.name = coordinate + ":" + transform.parent.GetComponent<Island>().islandName;

        if (tileHarvest==null)
        {
            tileHarvest = new UnityEvent();
        }
        
        if (tilePlant==null)
        {
            tilePlant = new UnityEvent();
        }

        if (tileStatusCahnged == null)
        {
            tileStatusCahnged = new UnityEvent();
        }

        plantPos = transform.GetChild(0);
        meshFilter = transform.GetChild(1).GetComponent<MeshFilter>();
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = 6f;
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        defY = transform.position.y;
        UpperY = defY + 0.4f;

    }

    private void Start()
    {
        defScale = transform.GetChild(1).localScale;

        if (transform.parent.GetComponent<Island>().islandName == "Mashroom" || transform.parent.GetComponent<Island>().islandName == "Restaurant")
        {
            //gameObject.SetActive(false);

            MeshRenderer[] meshRendererNew = GetComponentsInChildren<MeshRenderer>();
            GetComponent<TilePart>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;


            foreach (var mesh in meshRendererNew)
            {
                mesh.enabled = false;
            }
        }        

        if(isRestaurantTile)
        {
            transform.GetChild(1).gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            defScale = transform.GetChild(1).localScale;
            transform.GetChild(1).localPosition = new Vector3(meshFilter.transform.localPosition.x, meshFilter.transform.localPosition.y, meshFilter.transform.localPosition.z);
        }

        if (transform.parent.GetComponent<Island>().islandName == "Island" && statusIndex == 0)
        {
            RemovebleObjectsSpawn();
        }
    }
    
    private void RemovebleObjectsSpawn()
    {
        if (transform.GetComponentsInChildren<Tool>() == null)
        {
            int r = UnityEngine.Random.Range(0, 100);
            if (r < 50)
            {
                GameObject cloneRemovebleObject = Instantiate(GameManager.gameManager.RemovebleObjects, transform);
        
                if (r < 25)
                {
                    List<Transform> rocks = new List<Transform>(cloneRemovebleObject.transform.GetChildsWhere(p => p.name == "Rock"));
                    rocks[UnityEngine.Random.Range(0, rocks.Count)].gameObject.SetActive(true);
                    cloneRemovebleObject.transform.localPosition = Vector3.zero;
                }
                else if (r > 25 && r < 50)
                {
                    List<Transform> grass = new List<Transform>(cloneRemovebleObject.transform.GetChildsWhere(p => p.name == "Grass"));
                    grass[UnityEngine.Random.Range(0, grass.Count)].gameObject.SetActive(true);
                    cloneRemovebleObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (GameManager.gameManager.isActive && !isRestaurantTile)
        {
            SelectEffect();
        }


        GameManager.gameManager.currentTilePart = this;
        GameManager.gameManager.tileChange.Invoke();
    }

    private void OnMouseDown()
    {
        if (GameManager.gameManager.isActive && childPlant != null && childPlant.GetComponent<Plant>())
        {
            if (!childPlant.GetComponent<Plant>().readyHarvest)
                HarvestAnimation();
        }
    }
    
    public void SelectEffect()
    {
        transform.DOMoveY(UpperY, 0.2f);
        outline.enabled = true;       
    }

    public void UnselectEffect()
    {
        isHoverOpened = false;
        transform.DOMoveY(defY, 0.2f);
        outline.enabled = false;
    }

    private void OnMouseExit()
    {
        if (GameManager.gameManager.isActive || !isRestaurantTile)
        {
            UnselectEffect();
            overClone = 0;
            UIManager.uiManager.hoverPanel.SetActive(false);
            UIManager.uiManager.hoverText.text = "";
        }

        GameManager.gameManager.currentTilePart = null;
        GameManager.gameManager.tileChange.Invoke();
    }

    private void Update()
    {
        IconCheck();

        if(hover)
        {
            OnMouseOver();
        }
    }

    bool isHoverOpened = false;

    private void OnMouseOver()
    {
        if (!GameManager.gameManager.isActive || isRestaurantTile)
            return;
        
        if (overClone < overTrigger)
        {
            overClone += Time.deltaTime;
        }        
        else if(isHoverOpened==false)
        {
            isHoverOpened = true;
            Hover _hoverPanel = UIManager.uiManager.hoverPanel.GetComponent<Hover>();
            _hoverPanel.ClearStatus();
            List<TileBuff> ownedBuffs = new List<TileBuff>();
            ownedBuffs.AddRange(GetComponents<TileBuff>());
            
            if (childPlant!=null)
            {
                if (childPlant.GetComponent<Plant>())
                {
                    Plant clonePlant = childPlant.GetComponent<Plant>();
                    _hoverPanel.FillNewData(clonePlant.plantSprite, clonePlant.parentCard.name, clonePlant.level.ToString(), childPlant);
                    if (childPlant.GetComponent<Plant>().isGrowth)
                    {
                        _hoverPanel.AddStatus(LeanLocalization.GetTranslationText("ReadyToHarvest"), false);
                    }
                    else
                    {
                        LeanLocalization.SetToken("GROWTH_DAY", clonePlant.remainingDayCount.ToString());                        
                        _hoverPanel.AddStatus(LeanLocalization.GetTranslationText("XDayToGrowth"), false);
                    }

                    if (statusIndex == 2)
                    {
                        _hoverPanel.AddStatus(LeanLocalization.GetTranslationText("NeedWater"), true);
                    }
                }
                else if(childPlant.gameObject.GetComponent<Tool>())
                {
                    Tool cloneTool = childPlant.gameObject.GetComponent<Tool>();
                    _hoverPanel.FillNewData(cloneTool.parentCard.icon, LeanLocalization.GetTranslationText(cloneTool.parentCard.name), cloneTool.Level.ToString(),childPlant);
                }
                
                else if (childPlant.gameObject.GetComponent<Building>())
                {
                    Building cloneTool = childPlant.gameObject.GetComponent<Building>();
                    _hoverPanel.FillNewData(cloneTool.parentCard.icon, LeanLocalization.GetTranslationText(cloneTool.parentCard.name),"", childPlant.gameObject);
                    _hoverPanel.AddStatus(LeanLocalization.GetTranslationText(childPlant.GetComponent<Building>().description), false);
                }
                
                else if (childPlant.gameObject.GetComponent<Debuff>())
                {
                    Debuff cloneDebuff = childPlant.gameObject.GetComponent<Debuff>();
                    _hoverPanel.FillNewData(cloneDebuff.icon, LeanLocalization.GetTranslationText(cloneDebuff.name)+" !", "", childPlant.gameObject);

                    _hoverPanel.AddStatus(LeanLocalization.GetTranslationText(childPlant.gameObject.GetComponent<Debuff>().description), true);
                }
            }
            else
            {
                _hoverPanel.FillNewData(tilePartIcon, LeanLocalization.GetTranslationText("Empty"),"",childPlant);
            }


            if (ownedBuffs.Count != 0)
            {
                for (int i = 0; i < ownedBuffs.Count; i++)
                {
                    _hoverPanel.AddStatus(LeanLocalization.GetTranslationText(ownedBuffs[i].description) ,false);
                }
            }
            
            //_hoverPanel.gameObject.SetActive(true);
        }
    }

    public void GoStartPos()
    {
        transform.DOMoveY(defY, 0.3f);
    }

    public void HarvestAnimation()
    {
        //transform.DOMoveY(UpperY, 0.2f);
        outline.enabled = true;
        tileHarvest.Invoke();
        //transform.DOMoveY(defY, 0.2f);
        CameraShake.camerShake.Shake(0.4f,0.2f);

        Plant previousPlant;

        if (childPlant != null && childPlant.GetComponent<Plant>() && childPlant.GetComponent<Plant>().isGrowth)
        {   
            previousPlant = childPlant.GetComponent<Plant>();
            childPlant.GetComponent<Plant>().ReadytoHarvest();


            GameObject cloneObject = Instantiate(previousPlant.parentCard.spawnObject, plantPos.position,
               previousPlant.parentCard.spawnObject.transform.rotation,
               GameManager.gameManager.currentTilePart.transform);
            
            tilePlant.Invoke();
            childPlant = cloneObject;
            AudioManager.audioManager.ChangeAudioClip(0);
            AudioManager.audioManager.TriggerActiveClip();

            Instantiate(GameManager.gameManager.plantFX, GameManager.gameManager.currentTilePart.plantPos.position, GameManager.gameManager.plantFX.transform.rotation);
        }

        outline.enabled = false;
    }

    public void HarvestAnimation(bool ignore)
    {
        //transform.DOMoveY(UpperY, 0.2f);
        outline.enabled = true;
        tileHarvest.Invoke();
        //transform.DOMoveY(defY, 0.2f);
        CameraShake.camerShake.Shake(0.4f, 0.2f);
        if (childPlant != null && childPlant.GetComponent<Plant>() && childPlant.GetComponent<Plant>().isGrowth)
        {
            isPlanted = false;

            if(!ignore)
                childPlant.GetComponent<Plant>().ReadytoHarvest();
        }
        outline.enabled = false;
    }

    public IEnumerator WaveAnim()
    {   
        transform.DOMoveY(UpperY, 0.2f);
        outline.enabled = true;
        yield return new WaitForSeconds(0.2f);
        transform.DOMoveY(defY, 0.2f);
        outline.enabled = false;
    }

    public void ChangeStatus(int newStatus)
    {
        statusIndex = newStatus;
        meshFilter.sharedMesh = GameManager.gameManager.tilePartStatus.Find(p => p.bigIslandName == transform.parent.GetComponent<Island>().islandName).tileParts[statusIndex].tilePartMesh;
        Material mat = GameManager.gameManager.tilePartStatus.Find(p=> p.bigIslandName == transform.parent.GetComponent<Island>().islandName).tileParts[statusIndex].material;
        if (mat != null) 
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial = mat;

        tileStatusCahnged.Invoke();
    }

    private void CheckStatusNewDay()
    {
        if (isPlanted && childPlant!=null &&childPlant.GetComponent<Plant>())
        {
            childPlant.GetComponent<Plant>().Growth();
        }

        if (statusIndex==3)
        {
            ChangeStatus(2);
            Debug.Log("Tile status 2! : " + gameObject.name);
        }
        
        else if (statusIndex==2 && !isPlanted)
        {            
        }
        
    }

    public void ExecuteAllBuffs()
    {
        List<TileBuff> allBuffs = new List<TileBuff>();
        allBuffs.AddRange(GetComponents<TileBuff>());
        for (int i = 0; i < allBuffs.Count; i++)
        {
            allBuffs[i].Execute();
        }
    }

    private void IconCheck()
    {
        List<TileBuff> tileBuffs = new List<TileBuff>();
        tileBuffs.AddRange(GetComponents<TileBuff>());
        List<TileBuff> cloneTileBuffs = new List<TileBuff>(tileBuffs.Where(p=> p.name != "Springler Buff"));

        if (childPlant != null && isPlanted && statusIndex==2 && childPlant.GetComponent<Plant>() && !childPlant.GetComponent<Plant>().isGrowth)
        {
            iconImage.sprite = waterIcon;
            uiPanel.SetActive(true);   
        }        
        
        else if (childPlant != null && isPlanted && childPlant.GetComponent<Plant>() && childPlant.GetComponent<Plant>().isGrowth)
        {
            iconImage.sprite = plantGrowth;
            uiPanel.SetActive(true);
        }
        
        else
        {
            uiPanel.SetActive(false);
        }
    }

    /*public void Plant(GameObject plant, int phase, int level)
    {
        
    }*/

    private void OnEnable()
    {
        //StartCoroutine(WaveAnim());

        if (isRestaurantTile && (transform.parent.GetComponent<Island>().IsActive || transform.parent.GetComponent<Island>().isMain))
        {
            if (gridPoints.Count != 0)
                return;

            for (int a = 0; a < 3; a++)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject clone = Instantiate(gridPoint, Vector3.zero, Quaternion.identity, transform);
                    clone.transform.localPosition = new Vector3((gridSize * i) - 0.5f, 1f, (gridSize * a) - 0.5f);
                    gridPoints.Add(clone.GetComponent<GridPointCube>());
                    clone.transform.SetParent(null, true);
                    clone.name = this.name + new Vector3((gridSize * i) - 0.5f, 1f, (gridSize * a) - 0.5f);
                }
            }
        }
    }

    public void LoadData(GameData data)
    {        
        GameObject childPlantClone = null;
        SpawnedPlant plant = data.SpawnedPlants.Find(p=> p.parentName == gameObject.name);
        SpawnedBuilding building = data.SpawnedBuildings.Find(p => p.parentName == gameObject.name);
        SpawnedTool tool = data.SpawnedTools.Find(p => p.parentName == gameObject.name);
        DebuffObjects debuff = data.spawnedDebuff.Find(p => p.parentName == gameObject.name);
        
        if (plant==null && building==null && tool ==null && debuff==null)
            return;

        if (plant!=null)
        {
            Card cloneCard = Instantiate(GameManager.gameManager.AllCards.Find(p => p.cardID == plant.cardID));
            Plant cloneObject = Instantiate(cloneCard.spawnObject, plantPos.position,cloneCard.spawnObject.transform.rotation , transform).GetComponent<Plant>();
            cloneObject.currentDayCount = plant.CurrentDayCount;
            cloneObject.level = plant.level;
            cloneObject.isGrowth = plant.isGrowth;
            cloneObject.currentDayCount = plant.CurrentDayCount;
            childPlantClone = cloneObject.gameObject;
        }

        else if(building!=null)
        {
            Card cloneCard = Instantiate(GameManager.gameManager.AllCards.Find(p => p.cardID == building.cardID));
            Building cloneObject = Instantiate(cloneCard.spawnObject, plantPos.position,cloneCard.spawnObject.transform.rotation, transform)
                .GetComponent<Building>();

            cloneObject.currentAnimalCount = building.currentAnimalCount;
            childPlantClone = cloneObject.gameObject;
        }
        
        else if(debuff!=null)
        {
            Debuff cloneObject = Instantiate(GameManager.gameManager.debuffs.Find(p => p.name == debuff.debuffName),
                plantPos.position, plantPos.transform.rotation, transform);
            
            childPlantClone = cloneObject.gameObject;
        }

        else if(tool!=null)
        {
            Card cloneCard = Instantiate(GameManager.gameManager.AllCards.Find(p => p.cardID == tool.cardID));
            Tool cloneObject = Instantiate(cloneCard.spawnObject, plantPos.position,cloneCard.spawnObject.transform.rotation, transform)
                .GetComponent<Tool>();
            
            childPlantClone = cloneObject.gameObject;
        }


        isPlanted = true;
        childPlant = childPlantClone;        
    }

    public void SaveData(ref GameData data)
    {
        SpawnedPlant testPlant = data.SpawnedPlants.Find(p=> p.parentName == gameObject.name);
        SpawnedBuilding testBuilding = data.SpawnedBuildings.Find(p => p.parentName == gameObject.name);
        SpawnedTool testTool = data.SpawnedTools.Find(p => p.parentName == gameObject.name);
        DebuffObjects testDebuff = data.spawnedDebuff.Find(p => p.parentName == gameObject.name);
        
        if (testPlant != null)
        {
            data.SpawnedPlants.Remove(testPlant);
        }
        if (testBuilding!=null)
        {
            data.SpawnedBuildings.Remove(testBuilding);
        }
        if (testTool != null)
        {
            data.SpawnedTools.Remove(testTool);
        }

        if (testDebuff!=null)
        {
            data.spawnedDebuff.Remove(testDebuff);
        }
        
        if (childPlant!=null)
        {
            if (childPlant.GetComponent<Plant>())
            {
                Plant childPlantClone = childPlant.GetComponent<Plant>();
                SpawnedPlant newPlantData = new SpawnedPlant();
            
                newPlantData.level = childPlantClone.level;
                newPlantData.isGrowth = childPlantClone.isGrowth;
                newPlantData.cardID = childPlantClone.parentCard.cardID;
                newPlantData.CurrentDayCount = childPlantClone.currentDayCount;
                newPlantData.parentName = gameObject.name;
            
                data.SpawnedPlants.Add(newPlantData);   
            }

            if (childPlant.GetComponent<Building>())
            {
                SpawnedBuilding building = new SpawnedBuilding();
                building.currentAnimalCount = childPlant.GetComponent<Building>().currentAnimalCount;
                building.cardID = childPlant.GetComponent<Building>().parentCard.cardID;
                building.parentName = gameObject.name;
                
                data.SpawnedBuildings.Add(building);
            }

            if (childPlant.GetComponent<Tool>())
            {
                SpawnedTool tool = new SpawnedTool();
                tool.parentName = gameObject.name;
                tool.cardID = childPlant.GetComponent<Tool>().parentCard.cardID;
                
                data.SpawnedTools.Add(tool);
            }

            if (childPlant.GetComponent<Debuff>())
            {
                DebuffObjects debuffObjects = new DebuffObjects();
                debuffObjects.debuffName = childPlant.GetComponent<Debuff>().name;
                debuffObjects.parentName = gameObject.name;
                
                data.spawnedDebuff.Add(debuffObjects);
            }
        }
    }
}
