using System.Collections.Generic;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    private Vector2 _velocity;
    public List<GameObject> _obstacles;

    [SerializeField] private float _minDetectionRange = 0.5f;

    void Start()
    {
        _velocity = GetComponent<PlayerController>()._velocity;
    }

    
    void Update()
    {
        
    }

    Vector2 ObstacleAvoidance()
    {
        float DetectionRange = _velocity.magnitude + _minDetectionRange;

        foreach(GameObject obstacle in _obstacles)
        {
            if(Vector2.Distance(transform.position,obstacle.transform.position) < DetectionRange)
            {
                obstacle.tag = "Marked";
            }
            else
            {
                obstacle.tag = "Untagged";
            }
        }

        GameObject ClosestObstacleInRange = null;
        float distToCOIR = 0;
        Vector2 COIRLocalPos;

        foreach(GameObject obstacle in _obstacles)
        {
            if(obstacle.tag == "Marked")
            {
                Vector2 locpos = transform.InverseTransformPoint(obstacle.transform.position);

                if(locpos.x >= 0)
                {

                }
            }
        }
    }
}
