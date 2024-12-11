using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springler : Tool
{
   public GameObject waterFX;
   public List<Vector2> targetTilesCoordinate = new List<Vector2>();
   private Vector2 parentTileCoordinate;
   public List<TilePart> targetTiles = new List<TilePart>();
   public TileBuff buff;
   private List<TileBuff> spawnedBuffs = new List<TileBuff>();

    private void Start()
   {
      base.Start();         
   }

   private void Update()
   {
      FindTargetTile();
   }

   private void FindTargetTile()
   {
      parentTileCoordinate = transform.parent.GetComponent<TilePart>().coordinate;
      for (int i = 0; i < targetTilesCoordinate.Count; i++)
      {
         GameObject cloneTile = GameObject.Find((parentTileCoordinate + targetTilesCoordinate[i]).ToString());
         if (cloneTile != null && !targetTiles.Contains(cloneTile.GetComponent<TilePart>()))
         {
            targetTiles.Add(cloneTile.GetComponent<TilePart>());
            spawnedBuffs.Add(CopyComponent(buff, cloneTile));
         }
      }
   }
   
   public override void Execute()
   {
      StartCoroutine(Cooldown());
   }

   private IEnumerator Cooldown()
   {
      yield return new WaitForSeconds(2f);
      
      for (int i = 0; i < targetTiles.Count; i++)
      {
         if (targetTiles[i].isPlanted && targetTiles[i].childPlant!=null &&
             (targetTiles[i].childPlant.GetComponent<Building>() || targetTiles[i].childPlant.GetComponent<Tool>()))
         {
            
         }

         else
         {
            targetTiles[i].ChangeStatus(3);
            Debug.Log("Changed : " + targetTiles[i].name);
            Vector3 targetPos = targetTiles[i].gameObject.transform.position;
            Instantiate(waterFX, new Vector3(targetPos.x, targetPos.y + 1, targetPos.z), waterFX.transform.rotation);
         }
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

    private void OnDestroy()
    {
        for (int i = 0; i < spawnedBuffs.Count; i++)
        {
            Destroy(spawnedBuffs[i]);
        }

        GameManager.gameManager.nextDay.RemoveListener(Execute);
    }
}
