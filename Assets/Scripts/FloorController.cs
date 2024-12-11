using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[Serializable]
public class FloorItem
{
    public int unlockLevel;
    public int floorIndex;
    public string name;
    public List<GameObject> floorPieces = new List<GameObject>();
    public ShopData shopData;
}

public class FloorController : MonoBehaviour
{
    public static FloorController floorController;
    public int currentFloorIndex;
    public int floorCount;
    public GameObject arrow;
    public GameObject dot;
    public Transform floorLayerGroup;
    public List<Image> images = new List<Image>();
    public Color activeDotColor;
    public List<FloorItem> floorItems = new List<FloorItem>();
    public List<FloorItem> activeFloorItems = new List<FloorItem>();
    public Animator waterAnimator;
    public bool isActive = true;

    public GameObject downArrow, upArrow;

    public bool rotateAnim;

    public CinemachineVirtualCamera mainVirtualCamera;
    private float defaultCameraDistince;
    public float cameraDistince;
    private float _cameraDistince;
    public float speed;

    private bool isRestaurant = false;
    private bool isReturnDefault = false;
    [HideInInspector] public bool isRestaurantOpen = false;
    public GameObject restaurantBackPanel;

    public GameObject islandTools;
    public GameObject siloPanel;

    private void Awake()
    {
        floorController = this;
    }

    private void Start()
    {
        GameManager.gameManager.farmLevelUp.AddListener(CreateFloorUI);
        defaultCameraDistince = mainVirtualCamera.m_Lens.OrthographicSize;
        _cameraDistince = cameraDistince;
    }

    private void Update()
    {
        if (isRestaurant)
        {
            if (cameraDistince < defaultCameraDistince)
            {
                defaultCameraDistince -= (Time.deltaTime * speed);
                mainVirtualCamera.m_Lens.OrthographicSize = defaultCameraDistince;
            }
            else
            {
                isRestaurant = false;
                defaultCameraDistince = 5f;
            }
        }

        if (isReturnDefault)
        {
            if (defaultCameraDistince > cameraDistince)
            {
                cameraDistince += (Time.deltaTime * speed);
                mainVirtualCamera.m_Lens.OrthographicSize = cameraDistince;
            }
            else
            {
                isReturnDefault = false;
                cameraDistince = _cameraDistince;
            }
        }
    }

    public void CreateFloorUI()
    {
        for (int i = 0; i < images.Count; i++)
        {
            Destroy(images[i].gameObject);
        }

        images.Clear();
        activeFloorItems.Clear();

        if(downArrow != null)
            Destroy(downArrow.gameObject);

        if (upArrow != null)
            Destroy(upArrow.gameObject);

        upArrow = Instantiate(arrow, Vector3.zero, Quaternion.identity, floorLayerGroup);
        upArrow.transform.eulerAngles = new Vector3(0, 0, 180);
        upArrow.GetComponent<Button>().onClick.AddListener(BackFloor);

        for (int i = 0; i < floorCount; i++)
        {
            if (floorItems[i].unlockLevel > GameManager.gameManager.farmLevel)
            {

            }
            else
            {
                GameObject cloneDot = Instantiate(dot, Vector3.zero, Quaternion.identity, floorLayerGroup);
                images.Add(cloneDot.GetComponent<Image>());
                activeFloorItems.Add(floorItems[i]);
                activeFloorItems.OrderBy(p => p.floorIndex);
            }
        }

        downArrow = Instantiate(arrow, Vector3.zero, Quaternion.identity, floorLayerGroup);
        downArrow.transform.eulerAngles = new Vector3(0, 0, 0);
        downArrow.GetComponent<Button>().onClick.AddListener(NextFloor);

        images[currentFloorIndex].color = activeDotColor;

        for (int i = 0; i < floorItems.Count; i++)
        {            
            floorItems[i].floorPieces.AddRange(GameManager.gameManager.bigIslandDatas.Find(p => p.name == floorItems[i].name).tileParts);            
        }
    }

    public void NextFloor()
    {
        if (!isActive || floorItems[currentFloorIndex].name == "Restaurant")
            return;

        int oldIndex;
        oldIndex = currentFloorIndex;

        if (activeFloorItems[currentFloorIndex + 1].name == "Restaurant")
        {
            OpenRestaurant();
            return;
        }

        if (floorItems[oldIndex].name == "Restaurant")
        {
            Restaurant.restourant.CloseRestaurant();
        }

        currentFloorIndex++;

        if (currentFloorIndex > images.Count-1)
        {
            currentFloorIndex = images.Count - 1;
            return;
        }

        images[oldIndex].color = Color.white;

        images[currentFloorIndex].color = activeDotColor;
        images[currentFloorIndex].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f).OnComplete(delegate
        {
            images[currentFloorIndex].transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        });

        isActive = false;
        StartCoroutine(FlipAnim(oldIndex));

        OnFloorChange();
    }

    void OnFloorChange()
    {
        if (floorItems[currentFloorIndex].name == "Mashroom")
        {
            waterAnimator.SetBool("Mashroom", true);
        }
        else
        {
            waterAnimator.SetBool("Mashroom", false);
        }

        if(floorItems[currentFloorIndex].name != "Restaurant")
        {
            for (int i = 0; i < UIManager.uiManager.hands.Count; i++)
            {
                UIManager.uiManager.hands[i].handTransform.gameObject.SetActive(UIManager.uiManager.hands[i].handName == floorItems[currentFloorIndex].name);
                GameManager.gameManager.handCardsChange.Invoke();
            }
        }
        

        UIManager.uiManager.shopPanel.SetActive(false);
        Shop.shop.ShopData = floorItems[currentFloorIndex].shopData;
        StartCoroutine(Silo.silo.FloorChanged());
    }

    private IEnumerator FlipAnim(int oldIndex)
    {
        for (int i = 0; i < activeFloorItems[currentFloorIndex].floorPieces.Count; i++)
        {
            if (activeFloorItems[currentFloorIndex].floorPieces[i].transform.parent.GetComponent<Island>().IsActive || activeFloorItems[currentFloorIndex].floorPieces[i].transform.parent.GetComponent<Island>().isMain)
            {
                Transform tileGraphicOld = activeFloorItems[currentFloorIndex].floorPieces[i].transform.GetChild(1).transform;
                Transform tileGraphic = activeFloorItems[currentFloorIndex].floorPieces[i].transform.GetChild(1).transform;                

                GameObject currentTilePart = activeFloorItems[currentFloorIndex].floorPieces[i];
                GameObject oldTilePart = activeFloorItems[oldIndex].floorPieces[i].gameObject;

                tileGraphicOld.DORotate(new Vector3(0, 0, -180f), 0.1f).OnComplete(delegate
                {

                    MeshRenderer[] meshRenderer = oldTilePart.GetComponentsInChildren<MeshRenderer>();
                    oldTilePart.GetComponent<TilePart>().enabled = false;
                    oldTilePart.GetComponent<Collider>().enabled = false;
                    if (oldTilePart.GetComponentInChildren<Canvas>())
                        oldTilePart.GetComponentInChildren<Canvas>().enabled = false;

                    foreach (var mesh in meshRenderer)
                    {
                        mesh.enabled = false;
                    }


                    tileGraphic.DORotate(new Vector3(0, 0, -180f), 0f);

                    MeshRenderer[] meshRendererNew = currentTilePart.GetComponentsInChildren<MeshRenderer>();
                    currentTilePart.GetComponent<TilePart>().enabled = true;
                    currentTilePart.GetComponent<Collider>().enabled = true;

                    if (currentTilePart.GetComponentInChildren<Canvas>())
                        currentTilePart.GetComponentInChildren<Canvas>().enabled = true;

                    foreach (var mesh in meshRendererNew)
                    {
                        mesh.enabled = true;
                    }

                    tileGraphic.transform.DORotate(new Vector3(0, 0, 0f), 0.06f);

                    tileGraphic.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.06f).OnComplete(delegate
                    {
                        tileGraphic.transform.DOScale(tileGraphic.parent.GetComponent<TilePart>().defScale, 0.06f);
                    });
                });

                yield return new WaitForSeconds(0.01f);

                /*floorItems[oldIndex].floorPieces[i].transform.DOMoveY(-2, 0.03f).OnComplete(delegate
                {
                    floorItems[oldIndex].floorPieces[i].gameObject.SetActive(false);
                });*/

                /*floorItems[currentFloorIndex].floorPieces[i].transform.DOMoveY(-2, 0f);
                floorItems[currentFloorIndex].floorPieces[i].SetActive(true);
                floorItems[currentFloorIndex].floorPieces[i].transform.DOMoveY(0, 0.04f);*/

            }
      
        }

        isActive = true;
    }

    public void BackFloor()
    {
        if (!isActive /*|| floorItems[currentFloorIndex].name == "Restaurant"*/)
            return;

        int oldIndex = currentFloorIndex;

        currentFloorIndex--;

        if (currentFloorIndex < 0)
        {
            currentFloorIndex = 0;
            return;
        }

        if (floorItems[oldIndex].name == "Restaurant")
        {
            Restaurant.restourant.CloseRestaurant();
        }

        if (floorItems[currentFloorIndex].name == "Mashroom")
        {
            UIManager.uiManager.restaurantPanel.GetComponent<Image>().enabled = false;
            UIManager.uiManager.shopButton.SetActive(true);
            restaurantBackPanel.SetActive(false);
            UIManager.uiManager.restaurantCustomizedWindow.TurnOff();
            UIManager.uiManager.restaurantMenu.TurnOff();
            UIManager.uiManager.openRestourantButton.SetActive(false);
            CameraController.cameraController.speed = CameraController.cameraController.defSpeed;
            isReturnDefault = true;
            islandTools.SetActive(true);
            siloPanel.SetActive(true);
            isRestaurantOpen = false;
            UIManager.uiManager.farmLevelSlider.gameObject.SetActive(true);
        }

        images[oldIndex].color = Color.white;
        images[currentFloorIndex].color = activeDotColor;
        images[currentFloorIndex].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f).OnComplete(delegate
        {
            images[currentFloorIndex].transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        });

        isActive = false;
        FlipAnim(oldIndex);

        OnFloorChange();

        StartCoroutine(FlipAnim(oldIndex));
    }

    public void BackFloor(int backFloorIndex)
    {
        if (!isActive /*|| floorItems[currentFloorIndex].name == "Restaurant"*/)
            return;

        int oldIndex = currentFloorIndex;

        currentFloorIndex = backFloorIndex;

        if (currentFloorIndex < 0)
        {
            currentFloorIndex = 0;
            return;
        }

        if (floorItems[currentFloorIndex].name == "Island")
        {
            UIManager.uiManager.floorPanel.SetActive(true);
            UIManager.uiManager.restaurantPanel.GetComponent<Image>().enabled = false;
            UIManager.uiManager.shopButton.SetActive(true);
            UIManager.uiManager.openRestourantButton.SetActive(false);
            restaurantBackPanel.SetActive(false);
            UIManager.uiManager.restaurantCustomizedWindow.TurnOff();
            UIManager.uiManager.restaurantMenu.TurnOff();
            CameraController.cameraController.speed = CameraController.cameraController.defSpeed;
            isReturnDefault = true;
            islandTools.SetActive(true);
            siloPanel.SetActive(true);
            isRestaurantOpen = false;
            UIManager.uiManager.farmLevelSlider.gameObject.SetActive(true);
        }

        images[oldIndex].color = Color.white;
        images[currentFloorIndex].color = activeDotColor;
        images[currentFloorIndex].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f).OnComplete(delegate
        {
            images[currentFloorIndex].transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        });

        isActive = false;
        FlipAnim(oldIndex);

        OnFloorChange();

        StartCoroutine(FlipAnim(oldIndex));
    }

    public void OpenRestaurant()
    {
        UIManager.uiManager.OpenLeanWindow(UIManager.uiManager.restaurantWindow);
    }

    public void GoToRestaurant()
    {

        int oldIndex = currentFloorIndex;
        currentFloorIndex = activeFloorItems.IndexOf(activeFloorItems.Find(p => p.name == "Restaurant"));

        images[oldIndex].color = Color.white;
        images[currentFloorIndex].color = activeDotColor;
        images[currentFloorIndex].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f).OnComplete(delegate
        {
            images[currentFloorIndex].transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        });

        isActive = false;
        FlipAnim(oldIndex);

        OnFloorChange();

        StartCoroutine(FlipAnim(oldIndex));


        foreach  (GameObject closedObject in UIManager.uiManager.closeOnMenu)
        {
            closedObject.SetActive(false);
        }

        UIManager.uiManager.restaurantPanel.SetActive(true);

        UIManager.uiManager.restaurantMenu.TurnOn();
        //UIManager.uiManager.restaurantCustomizedWindow.TurnOn();

         isRestaurant = true;
        CameraController.cameraController.speed = 0;
        islandTools.SetActive(false);
        restaurantBackPanel.SetActive(true);
        isRestaurantOpen = true;
        GameManager.gameManager.mouseClicked.RemoveAllListeners();
        CursorManager.cursorManager.index = 0;
        CursorManager.cursorManager.ChangeCursor();
        UIManager.uiManager.restaurantPanel.GetComponent<Image>().enabled = true;
        Menu.menu.FillMenu();

        List<GameObject> restaurantIsland = GameManager.gameManager.bigIslandDatas.Find(p => p.name == "Restaurant").islands.ToList();

        for (int i = 0; i < restaurantIsland.Count; i++)
        {
            List<CellData> gridCells = new List<CellData>(GridBuildingSystem.Instance.cellData.Where(p => p.parentIsland == restaurantIsland[i].GetComponent<Island>()));
            Debug.LogError(restaurantIsland[i].gameObject.name + restaurantIsland[i].GetComponent<Island>().IsActive);

            for (int a = 0; a < gridCells.Count; a++)
            {
                if (restaurantIsland[i].GetComponent<Island>().IsActive || restaurantIsland[i].GetComponent<Island>().isMain)
                {
                    gridCells[a].isActive = true;
                }

                else
                {
                    gridCells[a].isActive = false;
                }
            }
        }
    }
}
