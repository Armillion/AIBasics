using System;
using ImprovedTimers;
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
        
        private FrequencyTimer _spawnTimer;
        private int _spawnedZombies;

        private void Awake() {
            int ticksPerSecond = Mathf.CeilToInt(1f / _spawnRate);
            _spawnTimer = new FrequencyTimer(ticksPerSecond);
            _spawnTimer.OnTick += TrySpawnZombie;
            _spawnTimer.Start();
        }

        private void Start() {
            _zombieTemplate.gameObject.SetActive(false);
            
            for (var i = 0; i < _initialZombieCount; i++)
                SpawnZombie();
        }

        private void TrySpawnZombie() {
            if (Zombie.ZombieCount >= _maxZombieCount) return;
            SpawnZombie();
        }

        private void SpawnZombie() {
            Zombie zombie = Instantiate(_zombieTemplate, _arena.RandomSpawnablePoint(), Quaternion.identity);
            zombie.gameObject.SetActive(true);
            _spawnedZombies++;
            zombie.gameObject.name = $"Zombie {_spawnedZombies}";
        }
    }
}