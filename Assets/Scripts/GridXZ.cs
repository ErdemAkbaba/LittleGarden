using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class GridXZ<TGridObject>
{

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        

        gridArray = new TGridObject[width, height];

        List<int> widthSpecial = new List<int>();

        int islandIndex = 0;
        int f = 1;
        int l = 1;
        int h = 0;
        int k = 0;

        for (int i = 0; i < GridBuildingSystem.Instance.restaurantIsland.Count; i++)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                h++;
                if (h%(6*f) == 0)
                {
                    f++;
                }
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    k++;
                    gridArray[x, y] = createGridObject(this, x, y);
                    Vector2 cellCoordinate = new Vector2(x, y);
                    if (GridBuildingSystem.Instance.restaurantIsland.Count - 1 < islandIndex)
                    {
                        break;
                    }
                    Island parentIsland = GridBuildingSystem.Instance.restaurantIsland[islandIndex];
                    GridBuildingSystem.Instance.cellData.
                        Add(new CellData(cellCoordinate, GetWorldPosition(x, y), parentIsland.IsActive || parentIsland.isMain, false, null, parentIsland, null));
                    if (k%(6*l) == 0)
                    {
                        l++;
                        islandIndex++;
                        if (y == 17 && x < 5)
                        {
                            islandIndex = 0;
                            l = 1;
                            k = 0;
                        }
                        else if (y == 17 && x < 11) 
                        {
                            islandIndex = 3;
                            l = 1;
                            k = 0;
                        }
                        else if (y == 17 && x < 17)
                        {
                            islandIndex = 6;
                            l = 1;
                            k = 0;
                        }
                    }
                }
            }
        }


        /*for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            c++;

            if (c % 6 == 0)
            {
                islandIndex++;
            }

            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
                Vector2 cellCoordinate = new Vector2(x, z);
                Island parentIsland = GridBuildingSystem.Instance.restaurantIsland[islandIndex];
                GridBuildingSystem.Instance.cellData.
                    Add(new CellData(cellCoordinate, GetWorldPosition(x, z), parentIsland.isActive || parentIsland.isMain, false, null, parentIsland));
            }
        }*/

        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 5, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 10000f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 10000f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 10000f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 10000f);

            /*OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };*/
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, width - 1),
            Mathf.Clamp(gridPosition.y, 0, height - 1)
        );
    }

}
