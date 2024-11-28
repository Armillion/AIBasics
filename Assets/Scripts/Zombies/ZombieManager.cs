using System.Collections.Generic;
using System.Linq;
using SpacePartitioning;
using UnityEngine;
using Zombies.Environment;

namespace Zombies {
    public class ZombieManager : MonoBehaviour {
        [SerializeField, Min(0f)]
        private float _groupingRadius = 2f;
        
        [SerializeField, Min(1)]
        private int _attackGroupSize = 5;
        
        [SerializeField]
        private Arena _arena;
        
        public IReadOnlyCollection<Zombie> Zombies => _zombies;

        private readonly HashSet<Zombie> _zombies = new ();
        private CellSpacePartition<Zombie> _spacePartition;

        private void Start() {
            _spacePartition = new CellSpacePartition<Zombie>(_arena.Center, _arena.Size, _groupingRadius);
        }

        private void Update() {
            _spacePartition.UpdatePositions();

            foreach (Zombie zombie in _zombies) {
                HandleGrouping(zombie);
                zombie.GameTick();
            }
        }

        public void Register(Zombie zombie) {
            _zombies.Add(zombie);
            _spacePartition.Add(zombie);
        }

        public void Deregister(Zombie zombie) {
            _zombies.Remove(zombie);
            _spacePartition.Remove(zombie);
        }
        
        private void HandleGrouping(Zombie zombie) {
            int closeCount = _spacePartition.GetNearbyEntities(zombie.Position, _groupingRadius).Count(n => n != zombie);
            
            if (closeCount >= _attackGroupSize)
                zombie.IsChasingPlayer = true;
        }
    }
}