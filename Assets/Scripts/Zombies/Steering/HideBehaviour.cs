using System;
using KBCore.Refs;
using UnityEngine;
using Zombies.Environment;

namespace Zombies.Steering {
    [RequireComponent(typeof(ArriveBehaviour), typeof(EvadeBehaviour))]
    public class HideBehaviour : MonoBehaviour, ISteeringBehaviour {
        [SerializeField]
        private Component pursuerComponent;
        
        [SerializeField]
        private Arena _arena;
        
        [SerializeField, Min(0f)]
        private float _distanceFromCover = 1f;
        
        [SerializeField, Min(0f)]
        private float _threatDistance = 10f;
        
        [SerializeField, Self]
        private ArriveBehaviour _arriveBehaviour;
        
        [SerializeField, Self]
        private EvadeBehaviour _evadeBehaviour;
        
        private IVehicle Pursuer => pursuerComponent as IVehicle;

        private void OnValidate() {
            this.ValidateRefs();
            if (pursuerComponent && Pursuer != null) return;
            Debug.LogError($"{pursuerComponent} does not implement {nameof(IVehicle)}", this);
            pursuerComponent = null;
        }

        public Vector2 CalculateSteering(IVehicle vehicle) {
            if (Vector2.Distance(transform.position, Pursuer.Position) > _threatDistance)
                return Vector2.zero;
            
            var closestCoverDistance = float.MaxValue;
            Vector2 closestCoverPosition = Vector2.zero;
            
            foreach (Obstacle cover in _arena.Obstacles) {
                Vector2 hidingSpot = GetHidingSpot(cover.transform.position, cover.radius);
                float distanceToHidingSpot = Vector2.Distance(hidingSpot, transform.position);

                if (distanceToHidingSpot >= closestCoverDistance) continue;
                
                closestCoverDistance = distanceToHidingSpot;
                closestCoverPosition = hidingSpot;
            }
            
            if (Mathf.Approximately(closestCoverDistance, float.MaxValue)) {
                _evadeBehaviour.pursuer = Pursuer;
                return _evadeBehaviour.CalculateSteering(vehicle);
            }
            
            _arriveBehaviour.target = closestCoverPosition;
            return _arriveBehaviour.CalculateSteering(vehicle);
        }
        
        private Vector2 GetHidingSpot(Vector2 obstaclePosition, float obstacleRadius) {
            float distanceFromCenter = obstacleRadius + _distanceFromCover;
            Vector2 directionToObstacle = (obstaclePosition - Pursuer.Position).normalized;
            return directionToObstacle * distanceFromCenter + obstaclePosition;
        }
    }
}