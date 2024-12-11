using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCard : CardUIParent
{
    
    private List<TilePart> oldTargetTiles = new List<TilePart>();
    private int buffRate;
    public override void Start()
    {
        base.Start();
        GameManager.gameManager.tileChange.AddListener(SelectTargetTile);
        GameManager.gameManager.tileChange.AddListener(CheckBuffRate);
    }

    public override void PlaceCard()
    {
        base.PlaceCard();
        if (GameManager.gameManager.currentTilePart != null && GameManager.gameManager.currentTilePart.statusIndex == 3 && !GameManager.gameManager.currentTilePart.isPlanted)
        {
            if (cloneObject!=null)
            {
                Destroy(cloneObject.gameObject);
            }

            GameObject clonePlant = Instantiate(card.spawnObject, GameManager.gameManager.currentTilePart.plantPos.position,
                card.spawnObject.transform.rotation,
                GameManager.gameManager.currentTilePart.transform);
            GameManager.gameManager.currentTilePart.isPlanted = true;
            GameManager.gameManager.currentTilePart.tilePlant.Invoke(); 
            GameManager.gameManager.currentTilePart.childPlant = clonePlant;
            GameManager.gameManager.plants.Add(clonePlant.GetComponent<Plant>());
            AudioManager.audioManager.ChangeAudioClip(0);
            AudioManager.audioManager.TriggerActiveClip();
            SpawnBuffs(clonePlant.GetComponent<Plant>());
            ClearOldTiles();
            Destroy(this.gameObject);    
            
        }
        else
        {
            cardCover.SetActive(true);
            ClearOldTiles();
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouCanPlantJustWetPlowedTiles"));
            if (cloneObject!=null)
            {
                Destroy(cloneObject.gameObject);
            }
        }
    }
    
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
                        targetTile.GetComponent<TilePart>().outline.OutlineColor = Color.red;
                        targetTile.GetComponent<TilePart>().outline.OutlineMode = Outline.Mode.OutlineAll;
                        oldTargetTiles.Add(targetTile.GetComponent<TilePart>());
                    }
                }   
            }
        }
    }
    
    private void CheckBuffRate()
    {
        if (GameManager.gameManager.currentTilePart != null && cloneObject != null)
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
            cloneObject.GetComponent<SpawnClone>().buffText.text = "+" + buffRate.ToString();   
        }
    }
}
