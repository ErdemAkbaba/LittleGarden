using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public Color color;
    public float cubeSize = 0.2f;
    public int cubesInRow = 5;
    public float lifetime = 0.1f;
    float cubesPivotDistance;
    Vector3 cubesPivot;

    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;

    // Use this for initialization
    void Start()
    {
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.tag == "weapon" || other.gameObject.tag == "Player")
        {
            explode();
        }*/

    }

    public void explode()
    {
        //make object disappear
        //gameObject.SetActive(false);

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
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

        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders)
        {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

    }
    
    public void explode(Vector3 spawnPoint)
    {
        //make object disappear
        //gameObject.SetActive(false);

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z, spawnPoint);
                }
            }
        }

        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders)
        {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

    }

    void createPiece(int x, int y, int z)
    {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Renderer renderer = piece.GetComponent<Renderer>();
        renderer.material.color = color;
        
        //set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        piece.transform.rotation = new Quaternion(piece.transform.rotation.x,-45f, piece.transform.rotation.z, piece.transform.rotation.w);
        BoxCollider boxCollider = piece.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        piece.AddComponent<BoxCollider>();
        piece.gameObject.layer=7;
        lifetime=Random.Range(1,3);
        Destroy(piece, lifetime);
    }
    
    void createPiece(int x, int y, int z, Vector3 spawnPoint)
    {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Renderer renderer = piece.GetComponent<Renderer>();
        renderer.material.color = color;
        
        //set piece position and scale
        piece.transform.position = spawnPoint + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        piece.transform.rotation = new Quaternion(piece.transform.rotation.x,-45f, piece.transform.rotation.z, piece.transform.rotation.w);
        BoxCollider boxCollider = piece.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        piece.AddComponent<BoxCollider>();
        piece.gameObject.layer=7;
        lifetime=Random.Range(1,3);
        Destroy(piece, lifetime);
    }

}
