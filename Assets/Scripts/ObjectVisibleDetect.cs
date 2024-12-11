using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectVisibleDetect : MonoBehaviour
{
    private Camera camera;
    private Plane[] cameraFrustum;
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //CheckVisibleIslandPart();
    }

    public void CheckVisibleIslandPart()
    {
        if(!GameManager.gameManager.isActive)
            return;
        
        bool visibleTile=false;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        Bounds bounds;
        List<TilePart> islandParts = new List<TilePart>(
            GameManager.gameManager.spawnedTiles.Where(p=> p.gameObject.activeSelf));
        
        for (int i = 0; i < islandParts.Count; i++)
        {
            bounds = islandParts[i].gameObject.GetComponent<Collider>().bounds;

            if (GeometryUtility.TestPlanesAABB(cameraFrustum,bounds))
            {
                visibleTile = true;
            }
        }

        if (visibleTile==false)
        {
            //UIManager.uiManager.OpenGuidePanel();
        }
        else
        {
            UIManager.uiManager.CloseGuidePanel();
        }
    }
}
