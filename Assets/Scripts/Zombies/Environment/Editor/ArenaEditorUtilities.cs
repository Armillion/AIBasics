using UnityEditor;
using UnityEngine;

namespace Zombies.Environment.Editor {
    public partial class ArenaEditor {
        private Obstacle _obstacleTemplate;
        private float _minSize = 1f;
        private float _maxSize = 2f;
        private int _numberOfPrefabs = 1;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // Custom fields for spawning objects
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spawn Settings", EditorStyles.boldLabel);

            _obstacleTemplate = (Obstacle)EditorGUILayout.ObjectField(
                "Prefab to Spawn", _obstacleTemplate, typeof(Obstacle), true
            );

            EditorGUILayout.BeginHorizontal();
            _minSize = EditorGUILayout.FloatField("Min Size", _minSize);
            _maxSize = EditorGUILayout.FloatField("Max Size", _maxSize);
            EditorGUILayout.EndHorizontal();
            
            _numberOfPrefabs = EditorGUILayout.IntField("Number of Prefabs", _numberOfPrefabs);

            if (GUILayout.Button("Spawn Prefabs"))
                SpawnPrefabs();
        }

        private void SpawnPrefabs() {
            if (!_obstacleTemplate) {
                Debug.LogWarning("Please assign a prefab to spawn.");
                return;
            }

            for (var i = 0; i < _numberOfPrefabs; i++) {
                Obstacle spawnedObstacle = Instantiate(_obstacleTemplate);

                spawnedObstacle.transform.position = GetRandomPosition();
                float randomSize = Random.Range(_minSize, _maxSize);
                spawnedObstacle.radius = randomSize;

                Undo.RegisterCreatedObjectUndo(spawnedObstacle, "Spawn Prefab");
            }
        }

        private Vector3 GetRandomPosition() {
            var arena = (Arena)target;
            var bounds = (Bounds)_boundsField.GetValue(arena);
            
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0
            );
        }
    }
}