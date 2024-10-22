using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float obstacleRadius = 30f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleRadius);
    }
}
