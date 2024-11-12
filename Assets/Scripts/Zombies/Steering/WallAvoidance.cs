using NUnit.Framework;
using Shooter.Environment;
using UnityEngine;
using Zombies.Environment;

public class WallAvoidance : MonoBehaviour
{
    [SerializeField] private float feelerLength = 10f;
    [SerializeField] private float feelerAngle = 60f;

    [SerializeField] private Arena enviornment;

    private Vector2[] feelers = new Vector2[3];

    public Vector2 avoidWalls()
    {
        CreateFeelers();

        float IPdist = 0f;
        float closestIPdist = float.MaxValue;

        Vector2 closestWall = null;

        Vector2 steerForce = Vector2.zero, point, closestPoint;

        foreach(Vector2 feeler in feelers)
        {
            foundClosestWall = false;
            Vector2 previousWallPoint = null;

            foreach(Vector2 wallPoint in enviornment.Walls)
            {
                if (previousWallPoint != null)
                {
                    if (Utility.Geometry.LinesIntersect((Vector2)transform.position,feeler,previousWallPoint,wallPiont,point))
                    {
                        IPdist = Vector2.Distance((Vector2)transform.position,point);

                        if(IPdist < closestIPdist)
                        {
                            closestIPdist = IPdist;
                            closestPoint = point;
                            closestWall = wallPoint;
                        }
                    }
                }
            }

            if (foundClosestWall)
            {
                Vector2 overshoot = feeler.position - closestPoint;

                steerForce = overshoot; //needs to be multiplied by a wall normal
            }
        }

        return steerForce;
    }

    void CreateFeelers()
    {
        feelers[0] = (Vector2)transform.position + (Vector2.right*feelerLength);
        feelers[1] = (Vector2)transform.position + Utility.Geometry.Rotate((Vector2.right * feelerLength), feelerAngle);
        feelers[2] = (Vector2)transform.position + Utility.Geometry.Rotate((Vector2.right * feelerLength), -feelerAngle);
    }
}
