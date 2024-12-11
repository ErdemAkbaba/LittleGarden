using System.Collections.Generic;
using UnityEngine;

public class TestMouseCube : MonoBehaviour
{
    public Transform mousePos;
    [HideInInspector] public Vector3 targetVector3;
    public List<GridPointCube> transforms = new List<GridPointCube>();

    // Start is called before the first frame update
    void Start()
    {
        transforms.AddRange(GameObject.FindObjectsOfType<GridPointCube>());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 aim = Input.mousePosition;
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(aim);
        float hitDist = 0.0f;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            targetVector3 = targetPoint;
            mousePos.position = targetPoint;
        }

        transform.position = GetClosestEnemy(transforms).position;       
    }

    Transform GetClosestEnemy(List<GridPointCube> enemies)
    {
        GridPointCube bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = mousePos.position;
        foreach (GridPointCube potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget.transform;
    }
}
