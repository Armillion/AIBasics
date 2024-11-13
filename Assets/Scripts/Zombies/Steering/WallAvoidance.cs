using System;
using NUnit.Framework;
using UnityEngine;
using Zombies;
using Zombies.Environment;
using Zombies.Steering;

public class WallAvoidance : MonoBehaviour, ISteeringBehaviour
{
    [SerializeField] private float feelerLength = 10f;
    [SerializeField] private float feelerAngle = 60f;

    [SerializeField] private Arena enviornment;

    private Vector2[] feelers = new Vector2[3];

    public Vector2 CalculateSteering(IVehicle _)
    {
        CreateFeelers();

        float IPdist = 0f;
        float closestIPdist = float.MaxValue;

        (Vector2, Vector2)? closestWall = null;
        Vector2 steerForce = Vector2.zero;
        Vector2 point = Vector2.zero;
        Vector2 closestPoint = Vector2.zero;

        foreach(Vector2 feeler in feelers)
        {
            Vector2? previousWallPoint = null;

            foreach(Vector2 wallPoint in (Vector2[])enviornment.Walls)
            {
                if (previousWallPoint != null)
                {
                    if (Utility.Geometry.LinesIntersect((Vector2)transform.position, feeler, previousWallPoint.Value,wallPoint, out point))
                    {
                        IPdist = Vector2.Distance((Vector2)transform.position,point);

                        if(IPdist < closestIPdist)
                        {
                            closestIPdist = IPdist;
                            closestPoint = point;
                            closestWall = (previousWallPoint.Value, wallPoint);
                        }
                    }
                }

                previousWallPoint = wallPoint;
            }

            if (Utility.Geometry.LinesIntersect((Vector2)transform.position, feeler, previousWallPoint.Value, enviornment.Walls[0], out point))
            {
                IPdist = Vector2.Distance((Vector2)transform.position, point);

                if (IPdist < closestIPdist)
                {
                    closestIPdist = IPdist;
                    closestPoint = point;
                    closestWall = (previousWallPoint.Value, enviornment.Walls[0]);
                }
            }

            if (closestWall != null)
            {
                Vector2 overshoot = feeler - closestPoint;
                Vector2 direction = closestWall.Value.Item2 - closestWall.Value.Item1;
                Vector2 normal = new Vector2(-direction.y, direction.x).normalized;
                Debug.DrawRay(closestPoint, normal, Color.red);
                steerForce = overshoot.magnitude * normal; //needs to be multiplied by a wall normal
                Debug.Log(steerForce);
            }
            
        }

        Debug.DrawLine((Vector2)transform.position, transform.TransformPoint(steerForce), Color.green);
        return steerForce;
    }

    void CreateFeelers()
    {
        feelers[0] = (Vector2)transform.position + ((Vector2)transform.right*feelerLength);
        feelers[1] = (Vector2)transform.position + Utility.Geometry.Rotate(((Vector2)transform.right * feelerLength), feelerAngle);
        feelers[2] = (Vector2)transform.position + Utility.Geometry.Rotate(((Vector2)transform.right * feelerLength), -feelerAngle);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        
        foreach(Vector2 feeler in feelers)
            Gizmos.DrawLine(transform.position, feeler);
    }
}
