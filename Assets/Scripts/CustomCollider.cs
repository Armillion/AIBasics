using System.Collections.Generic;
using UnityEngine;
using Zombies;

public class CustomCollider : MonoBehaviour
{
    private Vector2 _velocity;
    public List<Obstacle> _obstacles;

    [SerializeField] private float _minDetectionRange = 0.5f;

    [SerializeField] private List<Obstacle> taggedObjects;
    [SerializeField] private Obstacle thisObstacle;

    void Start()
    {
        _velocity = GetComponent<IVehicle>().Velocity;
        taggedObjects = new List<Obstacle>();
        thisObstacle = GetComponent<Obstacle>();
    }

    
    void Update()
    {
        
    }

    Vector2 ObstacleAvoidance()
    {
        float DetectionRange = _velocity.magnitude + _minDetectionRange;

        foreach(Obstacle obstacleInstance in _obstacles)
        {
            if(Vector2.Distance(transform.position, obstacleInstance.gameObject.transform.position) < DetectionRange &&
               !taggedObjects.Contains(obstacleInstance))
            {
                taggedObjects.Add(obstacleInstance);
            }
            else if(taggedObjects.Contains(obstacleInstance))
            {
                taggedObjects.Remove(obstacleInstance);
            }
        }

        Obstacle ClosestObstacleInRange = null;
        float distToCOIR = float.MaxValue;   
        Vector2 COIRLocalPos = new Vector2;

        foreach(Obstacle obstacle in _obstacles)
        {
            if(taggedObjects.Contains(obstacle))
            {
                Vector2 locpos = transform.InverseTransformPoint(obstacle.transform.position);

                if(locpos.x >= 0)
                {
                    float expandedRadius = obstacle.obstacleRadius + thisObstacle.obstacleRadius;

                    if(Mathf.Abs(locpos.y) < expandedRadius)
                    {
                        float cX = locpos.x;
                        float cY = locpos.y;

                        float sqrtPart = Mathf.Sqrt(expandedRadius*expandedRadius - cY*cY);

                        float ip = cX - sqrtPart;

                        if(ip <= 0)
                        {
                            ip = cX + sqrtPart;
                        }

                        if(ip < distToCOIR)
                        {
                            distToCOIR = ip;
                            ClosestObstacleInRange = obstacle;
                            COIRLocalPos = locpos;
                        }
                    }
                }
            }
        }

        Vector2 steerForce = Vector2.zero;
        
        if(ClosestObstacleInRange != null)
        {
            float multiCulti = 1f + (DetectionRange - COIRLocalPos.x) / DetectionRange;
            steerForce.y = (ClosestObstacleInRange.obstacleRadius - COIRLocalPos.y) * multiCulti;
            steerForce.x = (ClosestObstacleInRange.obstacleRadius - COIRLocalPos.x) + 0.2f;
        }
        
        return transform.TransformVector(steerForce);
    }
}
