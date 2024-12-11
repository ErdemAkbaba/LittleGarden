using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomEvents : MonoBehaviour
{
    public static RandomEvents randomEvents;

    public List<GameObject> eventObjects = new List<GameObject>();
    public List<GameObject> activeEventObjects = new List<GameObject>();

    public float nextEventTime;

    public int maxEventCount;
    public int currentEventCount = 0;

    private void Awake()
    {
        randomEvents = this;
    }

    private void Start()
    {
        InvokeRepeating("GetEvent", 0, Random.Range(5, 20));
    }

    /*public void SpawnBirds(Plant plant, GameObject bird)
    {
        GameObject birdsClone = Instantiate(bird, airSpawnPos[Random.Range(0, airSpawnPos.Count)]);
        Vector3 targetPos = new Vector3(plant.transform.position.x, plant.transform.position.y + .75f, plant.transform.position.z);
        birdsClone.GetComponent<Animator>().SetTrigger("Fly");
        birdsClone.transform.LookAt(targetPos);
        Vector3 randomAirSpawnPos = airSpawnPos[Random.Range(0, airSpawnPos.Count)].position;
        birdsClone.transform.DOMove(targetPos, 5).OnComplete(delegate
        {
            plant.transform.parent.GetComponent<TilePart>().HarvestAnimation(true);
            plant.transform.parent = birdsClone.transform.Find("PlantHolder").transform;
            birdsClone.transform.LookAt(randomAirSpawnPos);
            birdsClone.transform.DOMove(randomAirSpawnPos, 5).OnComplete(delegate
            {
                Destroy(birdsClone);
            });
        });
    }*/

    public void SpawnMole(TilePart tilePart, GameObject mole)
    {
        //GameObject moleClone = Instantiate(mole, tilePart.transform);
        //moleClone.GetComponent<Animator>().SetTrigger("Fly");//todo
    }

    public void GetEvent()
    {
        if (maxEventCount == currentEventCount)
            return;

        List<GameObject> spawnable = new List<GameObject>(eventObjects);

        List<TilePart> plantTiles = new List<TilePart>(GameManager.gameManager.activeTiles.Where(p => p.childPlant));
        List<TilePart> growthPlants = new List<TilePart>(plantTiles.Where(p => p.childPlant.GetComponent<Plant>() && p.childPlant.GetComponent<Plant>().isGrowth == true));
        List<TilePart> removebleObjectTile = new List<TilePart>(GameManager.gameManager.activeTiles.Where(p => p.statusIndex == 0 && p.transform.Find("RemovebleEnvironment")));

        if (GameObject.FindAnyObjectByType<Mole>() && removebleObjectTile.Count > 0)
        {
            spawnable.Remove(spawnable.Find(p => p.GetComponent<Mole>()));
        }
        if (GameObject.FindAnyObjectByType<Bird>() || plantTiles.Count == 0 || growthPlants.Count == 0 || plantTiles.Where(p => p.GetComponent<Plant>().spawnedBuffs.Count > 0).Count() > 0)
        {
            Debug.Log("bird not spawned");
            spawnable.Remove(spawnable.Find(p => p.GetComponent<Bird>()));
        }

        int r = Random.Range(0, spawnable.Count);
        Instantiate(spawnable[r]);
        currentEventCount++;




        /*GameObject cloneActiveEventObject = Instantiate(activeEventObjects[Random.Range(0, activeEventObjects.Count)]);
        tileParts = GameManager.gameManager.activeTiles;

        if (cloneActiveEventObject.GetComponent<RandomEventObject>().eventType == RandomEventObject.EventType.Bird)
        {
            Plant plant;

            foreach (TilePart tilePart in tileParts)
            {
                if (tilePart.gameObject.GetComponentInChildren<Plant>() != null)
                {
                    if (tilePart.gameObject.GetComponentInChildren<Plant>().isGrowth)
                    {
                        plants.Add(tilePart.gameObject.GetComponentInChildren<Plant>());
                    }
                }
            }

            plant = plants[Random.Range(0, plants.Count)];

            if (plants.Count > 0)
            {
                SpawnBirds(plant, cloneActiveEventObject);
                currentEventCount++;
            }
        } 
        else if (cloneActiveEventObject.GetComponent<RandomEventObject>().eventType == RandomEventObject.EventType.Mole)
        {
            TilePart moleTilePart;

            foreach (TilePart tilePart in tileParts)
            {
                if (tilePart.statusIndex == 0)
                {
                    moleTileParts.Add(tilePart);
                }
            }

            moleTilePart = moleTileParts[Random.Range(0, moleTileParts.Count)];

            if (moleTileParts.Count > 0)
            {
                SpawnMole(moleTilePart, cloneActiveEventObject);
                currentEventCount++;                    
            }
        }

        GameObject cloneEventObject = Instantiate(eventObjects[Random.Range(0, eventObjects.Count)]);

        switch (cloneEventObject.GetComponent<RandomEventObject>().eventType)
        {
            case RandomEventObject.EventType.ButterFly:
                cloneEventObject.transform.parent = airSpawnPos[Random.Range(0, airSpawnPos.Count)];
                cloneEventObject.transform.localPosition = Vector3.zero;
                currentEventCount++;
                break;
            case RandomEventObject.EventType.Fish:
                cloneEventObject.transform.parent = waterSpawnPos[Random.Range(0, waterSpawnPos.Count)];
                cloneEventObject.transform.localPosition = Vector3.zero;
                currentEventCount++;
                break;
            default:
                break;
        }*/
    }
}
