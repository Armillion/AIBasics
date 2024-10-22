using System.Collections.Generic;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    private Vector2 _velocity;
    public List<GameObject> _obstacles;

    [SerializeField] private float _minDetectionRange = 0.5f;

    [SerializeField] private List<GameObject> taggedObjects;

    void Start()
    {
        _velocity = GetComponent<PlayerController>().Velocity;
        taggedObjects = new List<GameObject>();
    }

    
    void Update()
    {
        
    }

    Vector2 ObstacleAvoidance()
    {
        float DetectionRange = _velocity.magnitude + _minDetectionRange;

        foreach(GameObject obstacle in _obstacles)
        {
            if(Vector2.Distance(transform.position,obstacle.transform.position) < DetectionRange &&
               !taggedObjects.Contains(obstacle))
            {
                taggedObjects.Add(obstacle);
            }
            else if(taggedObjects.Contains(obstacle))
            {
                taggedObjects.Remove(obstacle);
            }
        }

        GameObject ClosestObstacleInRange = null;
        float distToCOIR = 0;
        Vector2 COIRLocalPos;

        foreach(GameObject obstacle in _obstacles)
        {
            if(taggedObjects.Contains(obstacle))
            {
                Vector2 locpos = transform.InverseTransformPoint(obstacle.transform.position);

                if(locpos.x >= 0)
                {

                }
            }
        }
        
        return Vector2.zero;
    }
}
