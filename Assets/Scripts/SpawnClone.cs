using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class SpawnClone : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    [HideInInspector] public Mesh mesh;
    public Color avaliable, disable;
    public TextMeshProUGUI buffText;
    [HideInInspector] public int targetTileStatus;
    public bool isRestaurantEnv;

    public List<GridPointCube> gridPoints = new List<GridPointCube>();


    public Transform mousePos;
    [HideInInspector] public Vector3 targetVector3;


    void Start()
    {
        if (isRestaurantEnv)
        {
            //GameManager.gameManager.tileChange.AddListener(ChangeGridPoints);
            gridPoints.AddRange(FindObjectsOfType<GridPointCube>());
            GameManager.gameManager.inputActions.Player.R.performed += RotateMeshRenderer();
        }

        GameManager.gameManager.currentSpawnClone = this;
    }

    bool rotationActive = true;
    private Action<InputAction.CallbackContext> RotateMeshRenderer()
    {
        return (c) =>
        {
            if (!rotationActive)
                return;

            rotationActive = false;
            Vector3 targetRotation = new Vector3(meshRenderer.transform.eulerAngles.x, meshRenderer.transform.eulerAngles.y + 90f, meshRenderer.transform.eulerAngles.z);
            
            meshRenderer.transform.DORotate(targetRotation, 0.2f).OnComplete(delegate
            {
                rotationActive = true;
            });
                      
        };        
    }

    private void OnDestroy()
    {
        GameManager.gameManager.currentSpawnClone = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 aim = Input.mousePosition;
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(aim);
        float hitDist = 0.0f;        

        if (GameManager.gameManager.currentTilePart!=null)
        {
            if (!isRestaurantEnv)
            {
                if (GameManager.gameManager.currentTilePart.statusIndex == targetTileStatus && !GameManager.gameManager.currentTilePart.isPlanted)
                {
                    meshRenderer.material.color = avaliable;
                }
                else
                {
                    meshRenderer.material.color = disable;
                }

                transform.position = GameManager.gameManager.currentTilePart.plantPos.position;
            }
            else
            {
                if (playerPlane.Raycast(ray, out hitDist))
                {
                    Vector3 targetPoint = ray.GetPoint(hitDist);
                    targetVector3 = targetPoint;
                    mousePos.position = targetPoint;
                }

                transform.position = GetClosestGridPoint(gridPoints);
            }
        }
    }

    public void FillMesh()
    {
        Debug.Log(mesh.name);
        Debug.Log(meshFilter.sharedMesh.name);
        meshFilter.sharedMesh = mesh;
    }

    Vector3 GetClosestGridPoint(List<GridPointCube> points)
    {
        GridPointCube bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = mousePos.position;
        foreach (GridPointCube potentialTarget in points)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return new Vector3(bestTarget.transform.position.x, 1.3f, bestTarget.transform.position.z);
    }
}
