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
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 avoidWalls()
    {
        CreateFeelers();

        float IPdist = 0f;
        float closestIPdist = float.MaxValue;

        int closestWall = -1;

        Vector2 steerForce, point, closestPoint;

        foreach(Vector2 feeler in feelers)
        {
            foreach(Vector2 wall in enviornment.Walls) 
            { 
                if(Utility.Geometry.LinesIntersect((Vector2)transform.position),feeler,)
            }
        }

        return Vector2.zero;
    }

    void CreateFeelers()
    {
        feelers[0] = (Vector2)transform.position + (Vector2.right*feelerLength);
        feelers[1] = (Vector2)transform.position + Utility.Geometry.Rotate((Vector2.right * feelerLength), feelerAngle);
        feelers[2] = (Vector2)transform.position + Utility.Geometry.Rotate((Vector2.right * feelerLength), -feelerAngle);
    }
}
