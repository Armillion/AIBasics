using ImprovedTimers;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Physics {
    internal static class PhysicsBootstrapper {
        private static PlayerLoopSystem s_PhysicsSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertSimplePhysics2D<FixedUpdate>(ref currentPlayerLoop, 0)) {
                Debug.LogWarning("Physics not initialized, unable to register SimplePhysics2D into the Update loop.");
                return;
            }
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            
            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveSimplePhysics2D<FixedUpdate>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    
                    SimplePhysics2D.Clear();
                }
            }
#endif
        }

        private static void RemoveSimplePhysics2D<T>(ref PlayerLoopSystem loop)
            => PlayerLoopUtils.RemoveSystem<T>(ref loop, in s_PhysicsSystem);

        private static bool InsertSimplePhysics2D<T>(ref PlayerLoopSystem loop, int index) {
            s_PhysicsSystem = new PlayerLoopSystem {
                type = typeof(SimplePhysics2D),
                updateDelegate = SimplePhysics2D.EnsureZeroOverlap,
                subSystemList = null
            };
            
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in s_PhysicsSystem, index);
        }
    }
}