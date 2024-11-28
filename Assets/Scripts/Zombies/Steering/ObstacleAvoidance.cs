using System;
using System.Collections.Generic;
using KBCore.Refs;
using SpacePartitioning;
using UnityEngine;
using Zombies;
using Zombies.Environment;
using Zombies.Steering;

[RequireComponent(typeof(Obstacle))]
public class ObstacleAvoidance : MonoBehaviour, ISteeringBehaviour {
    [SerializeField] private float _minDetectionRange = 0.5f;

    [SerializeField, Self] private Obstacle thisObstacle;
    [SerializeField] private Arena _arena;
    
    private static CellSpacePartition<Obstacle> s_SpacePartition;
    private static bool s_SpacePartitionUpdated;

    private void OnValidate() => this.ValidateRefs();
    
    private void Start() {
        s_SpacePartition ??= new CellSpacePartition<Obstacle>(_arena.Center, _arena.Size, _minDetectionRange);
        s_SpacePartition.Add(thisObstacle);
    }

    private void OnEnable() => s_SpacePartition?.Add(thisObstacle);
    private void OnDisable() => s_SpacePartition?.Remove(thisObstacle);

    public Vector2 CalculateSteering(IVehicle vehicle)
    {
        float DetectionRange = vehicle.Velocity.magnitude + _minDetectionRange;

        // HACK
        if (!s_SpacePartitionUpdated)
        {
            s_SpacePartition.UpdatePositions();
            s_SpacePartitionUpdated = true;
        }
        
        IEnumerable<Obstacle> taggedObjects = s_SpacePartition.GetNearbyEntities(vehicle.Position, DetectionRange);
        
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
        
        return transform.TransformVector(steerForce);
    }

    private void LateUpdate() => s_SpacePartitionUpdated = false;
}
