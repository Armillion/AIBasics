using System;
using DefaultNamespace;
using UnityEngine;

namespace Zombies.Steering {
    [RequireComponent(typeof(ArriveBehaviour), typeof(EvadeBehaviour))]
    public class HideBehaviour : MonoBehaviour, ISteeringBehaviour {
        [SerializeField]
        private PlayerController _player;
        
        [SerializeField, Min(0f)]
        private float _distanceFromCover = 1f;
        
        [SerializeField]
        private Transform[] _covers;
        
        private ArriveBehaviour _arriveBehaviour;
        private EvadeBehaviour _evadeBehaviour;

        private void Start() {
            _arriveBehaviour = GetComponent<ArriveBehaviour>();
            _evadeBehaviour = GetComponent<EvadeBehaviour>();
        }

        public Vector2 CalculateSteering(IVehicle vehicle) {
            var closestCoverDistance = float.MaxValue;
            Vector2 closestCoverPosition = Vector2.zero;
            
            foreach (Transform cover in _covers) {
                Vector2 hidingSpot = GetHidingSpot(cover.position, cover.localScale.x / 2f);
                float distanceToHidingSpot = Vector2.Distance(hidingSpot, transform.position);

                if (distanceToHidingSpot >= closestCoverDistance) continue;
                
                closestCoverDistance = distanceToHidingSpot;
                closestCoverPosition = hidingSpot;
            }
            
            if (Mathf.Approximately(closestCoverDistance, float.MaxValue)) {
                _evadeBehaviour.pursuer = _player;
                return _evadeBehaviour.CalculateSteering(vehicle);
            }
            
            _arriveBehaviour.target = closestCoverPosition;
            return _arriveBehaviour.CalculateSteering(vehicle);
        }
        
        private Vector2 GetHidingSpot(Vector2 obstaclePosition, float obstacleRadius) {
            float distanceFromCenter = obstacleRadius + _distanceFromCover;
            Vector2 directionToObstacle = (obstaclePosition - _player.Position).normalized;
            return directionToObstacle * distanceFromCenter + obstaclePosition;
        }
    }
}