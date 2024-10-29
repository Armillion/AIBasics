using System;
using UnityEngine;
using Zombies.Environment;

namespace Zombies {
    public class ZombieSpawner : MonoBehaviour {
        [SerializeField]
        private Arena _arena;
        
        [SerializeField]
        private Zombie _zombieTemplate;
        
        [Header("Spawning")]
        [SerializeField, Min(0)]
        private int _initialZombieCount = 20;
        
        [SerializeField, Min(0)]
        private int _maxZombieCount = 100;
        
        [SerializeField, Min(0f)]
        private float _spawnRate = 1f;
        
        private int _zombieCount;

        private void Start() {
            _zombieTemplate.gameObject.SetActive(false);
            
            for (var i = 0; i < _initialZombieCount; i++)
                SpawnZombie();
        }

        private void Update() {
            if (Time.time % _spawnRate == 0 && _arena.Obstacles.Count > 0 && _arena.Obstacles.Count < _maxZombieCount) {
                SpawnZombie();
            }
        }

        private void SpawnZombie() {
            Zombie zombie = Instantiate(_zombieTemplate, _arena.RandomSpawnablePoint(), Quaternion.identity);
            zombie.gameObject.SetActive(true);
            zombie.gameObject.name = $"Zombie {_zombieCount++}";
        }
    }
}