using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCoin : MonoBehaviour
{
   // private List<GameObject> ores = new List<GameObject>();
    public float cubeSize = 0.2f;
    public int cubesInRow = 5;
    float cubesPivotDistance;
    public GameObject pre;
    // Use this for initialization
    void Start()
    {
        //   ores = GameManager.gameManager.ores;
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    public void explode()
    {
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }
    }

    public void ExplodeAndDestroySelf()
    {
        explode();
        Destroy(this.gameObject);
    }

    void createPiece(int x, int y, int z)
    {
        GameObject piece;
        Vector3 targetTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        piece = Instantiate(pre, targetTransform,Quaternion.identity);
        piece.layer = 7;
        int xForce = Random.Range(100, 200);
        int zForce = Random.Range(100, 200);
        piece.GetComponent<Rigidbody>().AddForce(new Vector3(xForce,100,zForce));
    }

}
