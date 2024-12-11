
using DG.Tweening;
using Lean.Localization;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CellData
{
    public Vector2 cellCoordinate;
    public Vector3 cellWorldPosition;
    public bool isActive;
    public bool isObjectPlaced;
    public GameObject placedObject;
    public Island parentIsland;
    public Card objectCard;
    public PlacedObjectTypeSO.Dir dir;

    public CellData(Vector2 _cellCoordinate, Vector3 _cellWorldPosition, bool _isActive, bool _isObjectPlaced, GameObject _placedObject, Island _parentIsland, Card _objectCard)
    {
        cellCoordinate = _cellCoordinate;
        cellWorldPosition = _cellWorldPosition;
        isActive = _isActive;
        isObjectPlaced = _isObjectPlaced;
        placedObject = _placedObject;
        parentIsland = _parentIsland;
        objectCard = _objectCard;
    }
}

public class GridBuildingSystem : MonoBehaviour, IDataPersistance
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [HideInInspector] public GridXZ<GridObject> grid;
    public List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    public Card testCard;
    public PlacedObjectTypeSO placedObjectTypeSO;
    public Card placedObjectCard;
    private PlacedObjectTypeSO.Dir dir;

    public List<CellData> cellData;
    public List<Island> restaurantIsland = new List<Island>();

    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;

    [HideInInspector] public UnityEvent OnPlacedObject;

    private void Awake()
    {
        Instance = this;
       
        placedObjectTypeSO = null;// placedObjectTypeSOList[0];
        placedObjectCard = null;

        OnPlacedObject = new UnityEvent();
    }

    private void Start()
    {
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, this.transform.position, (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));

        for (int i = 0; i < comeFromData.Count; i++)
        {
            if (GameManager.gameManager.AllCards.Find(p => p.cardID == comeFromData[i].objectCard) != null)
            {
                Card clonePlacedObject = GameManager.gameManager.AllCards.Find(p => p.cardID == comeFromData[i].objectCard);

                placedObjectTypeSO = clonePlacedObject.placedObjectTypeSO;
                placedObjectCard = clonePlacedObject;
                RefreshSelectedObjectType();
                Build(comeFromData[i].cellWorldPosition, new Vector2Int((int)comeFromData[i].cellCoordinate.x, (int)comeFromData[i].cellCoordinate.y), comeFromData[i].dir);
            }
        }

        placedObjectTypeSO = null;

        Restaurant.restourant.CloseFurnitures();
    }

    public class GridObject
    {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject_Done placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

    }

    [HideInInspector] public bool canBuild = true;
    [HideInInspector] public bool isAddedReputation = false;
    public void Build()
    {
        if (placedObjectTypeSO != null)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
            // Test Can Build
            canBuild = true;
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            //startwith

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (cellData.Find(p => p.cellCoordinate == new Vector2(gridPosition.x, gridPosition.y)) == null || !grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild() || cellData.Find(p => p.cellCoordinate == new Vector2(gridPosition.x, gridPosition.y)).isActive == false)
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);               

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    foreach (CellData objectCellData in cellData)
                    {
                        if (objectCellData.cellCoordinate == gridPosition)
                        {
                            objectCellData.isObjectPlaced = true;
                            objectCellData.placedObject = placedObject.gameObject;
                            objectCellData.objectCard = placedObjectCard;
                            objectCellData.dir = dir;
                            //placedObject.transform.SetParent(objectCellData.parentIsland.transform, true);
                            Restaurant.restourant.furniture.Add(placedObject.gameObject);
                        }
                    }
                }

                Restaurant.restourant.AddReputation(placedObjectCard.reputationValue);

                placedObject.transform.DOScale(placedObject.transform.localScale * 1.3f, 0.3f).OnComplete(delegate
                {
                    placedObject.transform.DOScale(placedObject.transform.localScale / 1.3f, 0.3f).SetEase(Ease.OutBack);
                }).SetEase(Ease.InBack);

                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                OnPlacedObject.Invoke();

                //DeselectObjectType();
            }
            else
            {
                UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("YouCantPlaceHere"));
                // Cannot build here
                //UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
            }
        }
    }


    public void Build(Vector3 placedObjectWorldPosition, Vector2Int coordinate, PlacedObjectTypeSO.Dir _dir)
    {
        if (placedObjectTypeSO != null)
        {

            if (canBuild)
            {          
                PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, coordinate, _dir, placedObjectTypeSO);

                grid.GetGridObject(coordinate.x, coordinate.y).SetPlacedObject(placedObject);
                CellData objectCellData = cellData.Find(p => p.cellCoordinate == coordinate);

                if(objectCellData != null)
                {
                    objectCellData.isObjectPlaced = true;
                    objectCellData.placedObject = placedObject.gameObject;
                    objectCellData.objectCard = placedObjectCard;
                    //placedObject.transform.SetParent(objectCellData.parentIsland.transform, true);
                    Restaurant.restourant.furniture.Add(placedObject.gameObject);

                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                    OnPlacedObject.Invoke();
                }
            }
        }
    }

    public void CanBuildPreview()
    {
        if (placedObjectTypeSO != null && GameManager.gameManager.activeTiles != null)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
            // Test Can Build
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            Color color;
            bool canPlace = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (cellData.Find(p => p.cellCoordinate == new Vector2(gridPosition.x, gridPosition.y)) == null || !grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild() || cellData.Find(p => p.cellCoordinate == new Vector2(gridPosition.x, gridPosition.y)).isActive == false)
                {
                    color = Color.red;
                    color.a = 1f;
                    MeshRenderer ghostBuilding = GameObject.FindAnyObjectByType<BuildingGhost>().transform.GetComponentInChildren<MeshRenderer>();
                    ghostBuilding.sharedMaterial.color = color;
                    canPlace = false;
                }
            }
            if (canPlace)
            {
                color = Color.green;
                color.a = 1f;
                MeshRenderer ghostBuilding = GameObject.FindAnyObjectByType<BuildingGhost>().transform.GetComponentInChildren<MeshRenderer>();
                ghostBuilding.sharedMaterial.color = color;
            }
        }
    }
    bool isDestroyed = false;
    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Build();
        }*/
        CanBuildPreview();

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        /*if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { placedObjectTypeSO = placedObjectTypeSOList[6]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { placedObjectTypeSO = placedObjectTypeSOList[7]; RefreshSelectedObjectType(); }*/

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }


        if (Input.GetMouseButtonDown(1) && GameManager.gameManager.currentTilePart != null && !Restaurant.restourant.isRestaurantOpen)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            if (grid.GetGridObject(mousePosition) != null)
            {
                // Valid Grid Position
                PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                if (placedObject != null)
                {
                    placedObject.transform.DOScale(placedObject.transform.localScale * 1.4f, 0.3f).OnComplete(delegate
                    {
                        placedObject.transform.DOScale(Vector3.zero , 0.3f).SetEase(Ease.OutBack).OnComplete(delegate { placedObject.DestroySelf(); });
                    }).SetEase(Ease.InBack);

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                    isDestroyed = false;
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                        foreach (CellData objectCellData in cellData)
                        {
                            if (objectCellData.cellCoordinate == gridPosition)
                            {
                                if (isDestroyed == false)
                                {
                                    GameManager.gameManager.SpawnCardByCard(objectCellData.objectCard, true);

                                    if(Restaurant.restourant.furniture.Contains(placedObject.gameObject))
                                    {
                                        Restaurant.restourant.furniture.Remove(placedObject.gameObject);
                                    }

                                    isDestroyed = true;
                                }
                                objectCellData.isObjectPlaced = false;
                                objectCellData.placedObject = null;
                                objectCellData.objectCard = null;
                                OnPlacedObject.Invoke();
                            }
                        }
                    }
                }
            }
        }
    }

    public void DeselectObjectType()
    {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    public void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    private List<GridCellData> comeFromData = new List<GridCellData>();
    public void LoadData(GameData data)
    {
        comeFromData.Clear();
        comeFromData.AddRange(data.GridCellDatas);       
    }

    public void SaveData(ref GameData data)
    {
        data.GridCellDatas.Clear();

        for (int i = 0; i < cellData.Count; i++)
        {
            GridCellData gridCellData = new GridCellData();
            gridCellData.parentIsland = cellData[i].parentIsland;

            if (cellData[i].objectCard != null)
            {
                gridCellData.objectCard = cellData[i].objectCard.cardID;
                gridCellData.objectEulerAngles = cellData[i].placedObject.transform.localEulerAngles;
            }                

            else
                gridCellData.objectCard = "";

            gridCellData.cellWorldPosition = cellData[i].cellWorldPosition;
            gridCellData.cellCoordinate = cellData[i].cellCoordinate;
            gridCellData.isObjectPlaced = cellData[i].isObjectPlaced;
            gridCellData.dir = cellData[i].dir;

            data.GridCellDatas.Add(gridCellData);
        }
    }
}
