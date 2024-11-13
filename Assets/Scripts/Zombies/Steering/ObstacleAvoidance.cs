using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using Zombies;
using Zombies.Environment;
using Zombies.Steering;

[RequireComponent(typeof(Obstacle))]
public class ObstacleAvoidance : MonoBehaviour, ISteeringBehaviour {
    [SerializeField] private float _minDetectionRange = 0.5f;

    [SerializeField] private List<Obstacle> taggedObjects = new();
    [SerializeField, Self] private Obstacle thisObstacle;

    private void OnValidate() => this.ValidateRefs();

    public Vector2 CalculateSteering(IVehicle vehicle)
    {
        float DetectionRange = vehicle.Velocity.magnitude + _minDetectionRange;

        foreach(Obstacle obstacleInstance in Obstacle.all)
        {
            if(obstacleInstance == thisObstacle)
            {
                continue;
            }
            
            if(Vector2.Distance(transform.position, obstacleInstance.gameObject.transform.position) < DetectionRange)
            {
                if (!taggedObjects.Contains(obstacleInstance))
                {
                    taggedObjects.Add(obstacleInstance);
                }
            }
            else 
            {
                taggedObjects.Remove(obstacleInstance);
            }
        }

        Obstacle ClosestObstacleInRange = null;
        float distToCOIR = float.MaxValue;   
        Vector2 COIRLocalPos = Vector2.zero;

        foreach(Obstacle obstacle in taggedObjects)
        {
            Vector2 locpos = transform.InverseTransformPoint(obstacle.transform.position);

            if(locpos.x >= 0)
            {
                float expandedRadius = obstacle.radius + thisObstacle.radius;

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

        Vector2 steerForce = Vector2.zero;
        
        if(ClosestObstacleInRange != null)
        {
            float multiCulti = 1f + (DetectionRange - COIRLocalPos.x) / DetectionRange;
            steerForce.y = (ClosestObstacleInRange.radius - COIRLocalPos.y) * multiCulti;
            steerForce.x = (ClosestObstacleInRange.radius - COIRLocalPos.x) + 0.2f;
        }
        
        Debug.DrawLine(transform.position, transform.position + (Vector3)steerForce, Color.yellow, 0.1f);
        return transform.TransformVector(steerForce);
    }
}
