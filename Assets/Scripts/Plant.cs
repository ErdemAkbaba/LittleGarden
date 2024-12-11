using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Localization;
using MyBox;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Card;
using Random = UnityEngine.Random;

[Serializable]
public class PlantPhases
{
    public Mesh phaseMesh;
    public Material phasePalette;
    public int phaseRequestDay;
}

[Serializable]
public class TargetTile
{
    public Vector2 coordinate;
}

public class Plant : MonoBehaviour
{
    public static Plant plant;

    [HideInInspector] public bool canBirdSpawn;
    public GameObject worms,birds;
    public List<int> rewardExpValues = new List<int>();
    public int level;
    public int expValue;
    public int currentDayCount;
    public Card parentCard;
    public MeshFilter meshFilter;
    public MeshRenderer meshMaterial;
    public List<PlantPhases> phases = new List<PlantPhases>();
    public bool isGrowth;
    public string plantID;
    public GameObject questSprite;
    public GameObject harvestFX;
    public Sprite plantSprite;
    public List<TileBuff> spawnedBuffs = new List<TileBuff>(); 
    [HideInInspector] public int harvestCount;
    [HideInInspector] public int remainingDayCount;

    public float growthCooldown;
    public float defGrowthTime;
    public float evolveTime;    
    [HideInInspector] public int currentPhase = 1;

    public bool isLastPhaseDiffrent;

    [ConditionalField("isLastPhaseDiffrent", false, true)]
    public Material lastPhaseMaterial;
    [ConditionalField("isLastPhaseDiffrent", false, true)]
    public Mesh lastPhaseMesh;
    private void Awake()
    {
        plant = this;
    }

    private void Start()
    {
        defGrowthTime = growthCooldown;
        evolveTime = defGrowthTime / (phases.Count - 1);        
        expValue = rewardExpValues[level-1];
        remainingDayCount = phases[phases.Count - 1].phaseRequestDay;
        //GameManager.gameManager.nextDay.AddListener(Growth);
        if (!GameManager.gameManager.plants.Contains(this))
        {
            GameManager.gameManager.plants.Add(this);
        }
        meshFilter.sharedMesh = phases[0].phaseMesh;
        meshMaterial.sharedMaterial = phases[0].phasePalette;
        Debug.Log(this.transform.parent);
        
        
        PlantPhases clonePhase = phases.Find(p => p.phaseRequestDay == currentDayCount);
        if (clonePhase != null)
        {
            meshFilter.sharedMesh = clonePhase.phaseMesh;
            meshMaterial.sharedMaterial= clonePhase.phasePalette;
        }
        if (currentDayCount == phases[phases.Count-1].phaseRequestDay)
        {
            isGrowth = true;
        }

        List<TileBuff> buffs = new List<TileBuff>(gameObject.transform.parent.GetComponents<TileBuff>().Where(p => p.name == "Scarecrow Buff"));    
        if (buffs.Count!=0)
        {
            canBirdSpawn = false;
        }
        else
        {
            canBirdSpawn = true;
        }
    }


    float harvestCooldown = 0.5f; 
    private void Update()
    {
        if (this.transform.position.y < -.5f)
        {
            PlantHarvest();
        }

        if (growthCooldown > 0)
        {
            growthCooldown -= Time.deltaTime;
        }

        if(growthCooldown < defGrowthTime - (evolveTime * currentPhase) && !isGrowth)
        {
            Growth();            
        }

        if (readyHarvest)
        {
            if (harvestCooldown > 0)
            {
                harvestCooldown -= Time.deltaTime;
            }

            else
            {
                PlantHarvest();
            }
        }

        if (transform.localPosition.y < -5f)
        {
            PlantHarvest();            
        }
    }

    public void Growth()
    {
        currentPhase ++;
        int i = Random.Range(0, 101);

        /*if (i>2 && i<4)
        {
            GameObject wormsClone = Instantiate(worms, transform.parent.GetComponent<TilePart>().plantPos.position,
                GameManager.gameManager.trashPlant.transform.rotation
                , transform.parent);

            for (int w = 0; w < wormsClone.transform.childCount; w++)
            {
                wormsClone.transform.GetChild(w).GetChild(0).GetComponent<MeshRenderer>().enabled = transform.GetChild(0).GetComponent<MeshRenderer>().enabled;
            }

            transform.parent.GetComponent<TilePart>().childPlant = wormsClone;
            transform.parent.GetComponent<TilePart>().isPlanted = true;
            Destroy(gameObject);
        }*/

        if (transform.parent.GetComponent<TilePart>().statusIndex==2 && !isGrowth)
        {
            GameObject trashPlant = Instantiate(GameManager.gameManager.trashPlant, transform.parent.GetComponent<TilePart>().plantPos.position,
                GameManager.gameManager.trashPlant.transform.rotation
                , transform.parent);
            transform.parent.GetComponent<TilePart>().childPlant = trashPlant;
            
            trashPlant.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = transform.GetChild(0).GetComponent<MeshRenderer>().enabled;
            
            Destroy(gameObject);
        }
        
        if (currentDayCount < phases[phases.Count-1].phaseRequestDay)
        {
            currentDayCount++;
            remainingDayCount = phases[phases.Count - 1].phaseRequestDay - currentDayCount;
            PlantPhases clonePhase = phases.Find(p => p.phaseRequestDay == currentDayCount);
            if (clonePhase != null)
            {
                meshFilter.sharedMesh = clonePhase.phaseMesh;
                meshMaterial.sharedMaterial = clonePhase.phasePalette;
            }
        }

        if (currentDayCount == phases[phases.Count-1].phaseRequestDay)
        {
            isGrowth = true;
            
            if(GetComponent<Outline>())
                GetComponent<Outline>().enabled = true;

            if (animationStarted==false)
            {
                InvokeRepeating("GrowthAnimation",0f,2.1f);
            }
        }
    }

    [HideInInspector] public bool readyHarvest;

    public void ReadytoHarvest()
    {
        if (readyHarvest)
            return;

        if (isLastPhaseDiffrent)
        {
            meshFilter.sharedMesh = lastPhaseMesh;
            meshMaterial.sharedMaterial = lastPhaseMaterial;
        }

        readyHarvest = true;
        AudioManager.audioManager.ChangeAudioClip(0);
        AudioManager.audioManager.TriggerActiveClip();
        gameObject.AddComponent<Rigidbody>();

        if(transform.GetChild(1) != null)
            transform.GetChild(1).gameObject.SetActive(false);

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1;
        randomDirection.y = transform.position.y;
        transform.DOJump(randomDirection, 3, 1, 2);
        transform.DORotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), 2);
        gameObject.transform.GetComponent<Collider>().enabled = true;

    }

    private void OnMouseDown()
    {
        PlantHarvest();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sea") && readyHarvest)
        {
            PlantHarvest();
        }
    }

    public void PlantHarvest()
    {
        if (!readyHarvest)
            return;

        //StartCoroutine(SpawnComboText());
        GameObject popupCanvas = UIManager.uiManager.popupCanvas;
        

        if (transform.parent != null && transform.parent.GetComponent<TilePart>())
            Instantiate(GameManager.gameManager.harvestFX, transform.parent.GetComponent<TilePart>().plantPos.position,
                GameManager.gameManager.harvestFX.transform.rotation);
        
        //gameObject.GetComponent<Explosion>().explode();
        
        GameObject clonePopupCanvas= Instantiate(popupCanvas, new Vector3(transform.position.x,transform.position.y,transform.position.z), popupCanvas.transform.rotation);
        clonePopupCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = expValue.ToString() + "<color #FFB900> exp </color>";


        UIManager.uiManager.SpawnExpSprite(transform.position,expValue);

        //GameObject clone = GameManager.gameManager.SpawnCardByCard(parentCard, true);
        Silo.silo.CreateSiloItem(parentCard, this);

        /*for (int i = 1; i < level; i++)
        {
            //clone.GetComponent<CardUI>().LevelUp();
        }*/

        Destroy(gameObject);
    }

    public void SpawnQuestSprite(Transform target)
    {
        QuestSprite cloneSprite = Instantiate(questSprite,
                new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
                questSprite.transform.rotation)
            .GetComponent<QuestSprite>();
        
        cloneSprite.icon.sprite = plantSprite;
        cloneSprite.targetTransform = target;
        cloneSprite.TriggerFollow();
    }

    private void OnDestroy()
    {
        BeforeDestroy();
    }

    private void BeforeDestroy()
    {
        for (int i = 0; i < spawnedBuffs.Count; i++)
        {
            Destroy(spawnedBuffs[i]);
        }

        GameManager.gameManager.plants.Remove(this);
    }

    private IEnumerator SpawnComboText()
    {
        float yAxis = 3f;
        for (int i = 0; i < spawnedBuffs.Count; i++)
        {
            UIManager.uiManager.SpawnComboPopUp(LeanLocalization.GetTranslationText("combo"), new Vector3(transform.position.x, transform.position.y + yAxis, transform.position.z));
            yAxis += 0.5f;
            Debug.Log(yAxis);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool animationStarted;
    public void GrowthAnimation()
    {
        animationStarted = true;
        Vector3 defScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        transform.DOScale(new Vector3(defScale.x + 0.02f, defScale.y + 0.02f, defScale.z + 0.02f), 1f).OnComplete(
            delegate
            {
                transform.DOScale(defScale, 1f);
            });
    }
}
