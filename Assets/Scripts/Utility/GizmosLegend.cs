using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility {
    [InitializeOnLoad]
    public static class GizmosLegend {
        private static readonly Dictionary<MonoBehaviour, Dictionary<string, Color>> _legend = new ();

        private const int LABEL_HEIGHT = 20;
        private const int LABEL_WIDTH = 300;
        private const int LABEL_MARGIN = 10;
        
        static GizmosLegend() {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void AddLabel(MonoBehaviour obj, string label, Color color) {
            if (!_legend.ContainsKey(obj))
                _legend[obj] = new Dictionary<string, Color>();

            _legend[obj][label] = color;
        }
        
        public static void RemoveLabel(MonoBehaviour obj, string label) {
            if (!_legend.TryGetValue(obj, out Dictionary<string, Color> value))
                return;

            value.Remove(label);
        }
        
        public static void Unregister(MonoBehaviour obj) {
            _legend.Remove(obj);
        }

        private static void OnSceneGUI(SceneView sceneView) {
            var position = new Vector2(50, 20);
            var indentLevel = 0;
            Handles.BeginGUI();
            
            foreach ((MonoBehaviour obj, Dictionary<string, Color> labels) in _legend) {
                GUI.color = Color.white;
                GUI.Label(new Rect(position.x, position.y, LABEL_WIDTH, LABEL_HEIGHT), obj.name);
                position.y += LABEL_HEIGHT;
                indentLevel++;
                
                foreach ((string label, Color color) in labels) {
                    GUI.color = color;
                    GUI.Label(new Rect(position.x + LABEL_MARGIN * indentLevel, position.y * indentLevel, LABEL_WIDTH, LABEL_HEIGHT), label);
                    position.y += LABEL_HEIGHT;
                }
                
                indentLevel--;
            }
            
            Handles.EndGUI();
        }
    }
}