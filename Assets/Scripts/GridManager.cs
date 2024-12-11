using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager gridManager;
    [SerializeField] private int width, height;
    [SerializeField] private GameObject parentTile;
    private void Start()
    {
        gridManager = this;
        StartCoroutine(CreateGrid());
    }

    private void Update()
    {
    }

    private IEnumerator CreateGrid()
    {
        GameObject cloneTilePart;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TilePart cloneTile = Instantiate(parentTile, new Vector3(x,0,y),parentTile.transform.rotation).GetComponent<TilePart>();
                cloneTile.GetComponent<TilePart>().ChangeStatus(0); //set light

                if (cloneTile.statusIndex == 1 && ((cloneTile.coordinate.y % 2 == 0) && (cloneTile.coordinate.x % 2 != 0) || (cloneTile.coordinate.x % 2 == 0) && (cloneTile.coordinate.y % 2 != 0)))
                    cloneTile.GetComponent<TilePart>().ChangeStatus(1); //set dark

                cloneTile.name = new Vector2(x,y).ToString();
                cloneTile.GetComponent<TilePart>().coordinate = new Vector2(x, y);
                GameManager.gameManager.spawnedTiles.Add(cloneTile.GetComponent<TilePart>());
                
                cloneTile.GetComponent<TilePart>().defY = cloneTile.transform.position.y;
                cloneTile.transform.position += new Vector3(0, 10, 0);
                cloneTile.GetComponent<TilePart>().GoStartPos();

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
