using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chair : MonoBehaviour
{
    public Vector2 targetCoordinate;
    public Vector2 originCoordinate;
    [HideInInspector] public UnityEvent tableReady;
    Chairs chairs;
    private void Start()
    {
        originCoordinate = GridBuildingSystem.Instance.cellData.Find(p => p.placedObject == this.gameObject).cellCoordinate;

        Debug.Log(transform.localEulerAngles.y);
        switch (transform.localEulerAngles.y)
        {
            case 0:
                targetCoordinate = new Vector3(originCoordinate.x, originCoordinate.y - 1);
                break;

            case 90:
                targetCoordinate = new Vector3(originCoordinate.x - 1, originCoordinate.y);
                break;

            case 270:
                targetCoordinate = new Vector3(originCoordinate.x + 1, originCoordinate.y);
                break;

            case 180:
                targetCoordinate = new Vector3(originCoordinate.x, originCoordinate.y + 1);
                break;
        }

        chairs = new Chairs(this.gameObject, false);

        Restaurant.restourant.chairs.Add(chairs);

        GridBuildingSystem.Instance.OnPlacedObject.AddListener(Seatable);

        Seatable();
    }

    public void Seatable()
    {
        if (GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == targetCoordinate).isObjectPlaced && GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == targetCoordinate).objectCard.cardID.StartsWith("T"))
        {
            if ((GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x, originCoordinate.y - 1)).isObjectPlaced ||
                GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x, originCoordinate.y - 1)).isActive == false) &&
                (GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x - 1, originCoordinate.y)).isObjectPlaced ||
                GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x - 1, originCoordinate.y)).isActive == false) &&
                (GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x + 1, originCoordinate.y)).isObjectPlaced ||
                GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x + 1, originCoordinate.y)).isActive == false) &&
                (GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x, originCoordinate.y + 1)).isObjectPlaced ||
                GridBuildingSystem.Instance.cellData.Find(p => p.cellCoordinate == new Vector2(originCoordinate.x, originCoordinate.y + 1)).isActive == false))
            {
                chairs.isSeatable = false;
            }
            else
            {
                chairs.isSeatable = true;
            }
        }
        else
        {
            chairs.isSeatable = false;
        }
    }

    private void OnDestroy()
    {
        Restaurant.restourant.chairs.Remove(chairs);
    }
}
