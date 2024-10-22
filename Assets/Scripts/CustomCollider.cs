using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using Zombies;
using Zombies.Environment;
using Zombies.Steering;

[RequireComponent(typeof(Obstacle))]
public class CustomCollider : MonoBehaviour, ISteeringBehaviour {
    [SerializeField]
    private Arena arena;

    [SerializeField] private float _minDetectionRange = 0.5f;

    [SerializeField] private List<Obstacle> taggedObjects;
    [SerializeField, Self] private Obstacle thisObstacle;

    private void OnValidate() => this.ValidateRefs();

    void Start()
    {
        taggedObjects = new List<Obstacle>();
    }

    
    void Update()
    {
        
    }

    public Vector2 CalculateSteering(IVehicle vehicle)
    {
        float DetectionRange = vehicle.Velocity.magnitude + _minDetectionRange;

        foreach(Obstacle obstacleInstance in arena.Obstacles)
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
        Vector2 COIRLocalPos = Vector2.zero;

        foreach(Obstacle obstacle in arena.Obstacles)
        {
            if(taggedObjects.Contains(obstacle))
            {
                Vector2 locpos = transform.InverseTransformPoint(obstacle.transform.position);

                if(locpos.x >= 0)
                {
                    float expandedRadius = obstacle.Radius + thisObstacle.Radius;

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
            steerForce.y = (ClosestObstacleInRange.Radius - COIRLocalPos.y) * multiCulti;
            steerForce.x = (ClosestObstacleInRange.Radius - COIRLocalPos.x) + 0.2f;
        }
        
        Debug.DrawLine(transform.position, transform.position + (Vector3)steerForce, Color.green, 0.1f);
        return transform.TransformVector(steerForce);
    }
}
