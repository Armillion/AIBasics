using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float obstacleRadius = 3f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleRadius);
    }
}
