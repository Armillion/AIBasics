using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Zombies {
    public class ZombieManager : MonoBehaviour {
        [SerializeField, Min(0f)]
        private float _groupingRadius = 2f;
        
        [SerializeField, Min(1)]
        private int _attackGroupSize = 5;
        
        public IReadOnlyCollection<Zombie> Zombies => _zombies;
        public NativeArray<int> CloseCount { get; private set; }

        private readonly HashSet<Zombie> _zombies = new ();
        private NativeArray<Vector3> _zombiePositions;

        private void Update() {
            _zombiePositions = new NativeArray<Vector3>(_zombies.Count, Allocator.TempJob);
            CloseCount = new NativeArray<int>(_zombies.Count, Allocator.TempJob);

            var index = 0;
            
            foreach (Zombie zombie in _zombies)
                _zombiePositions[index++] = zombie.Position;
            
            CalculateGrouping();
            index = 0;

            foreach (Zombie zombie in _zombies) {
                zombie.IsChasingPlayer = CloseCount[index] >= _attackGroupSize;
                zombie.GameTick();
                index++;
            }
            
            _zombiePositions.Dispose();
            CloseCount.Dispose();
        }

        public void Register(Zombie zombie) => _zombies.Add(zombie);
        public void Deregister(Zombie zombie) => _zombies.Remove(zombie);

        private struct AttackGroupCheckJob : IJobParallelFor {
            [ReadOnly]
            public NativeArray<Vector3> zombiePositions;

            [ReadOnly]
            public float groupingRadius;

            public NativeArray<int> closeCount;

            public void Execute(int index) {
                for (var other = 0; other < zombiePositions.Length; other++) {
                    if (other == index) continue;

                    float distance = Vector3.Distance(zombiePositions[index], zombiePositions[other]);

                    if (distance <= groupingRadius)
                        closeCount[index]++;
                }
            }
        }

        private void CalculateGrouping() {
            var job = new AttackGroupCheckJob {
                zombiePositions = _zombiePositions,
                groupingRadius = _groupingRadius,
                closeCount = CloseCount
            };

            JobHandle handle = job.Schedule(_zombiePositions.Length, 64);
            handle.Complete();
        }
    }
}