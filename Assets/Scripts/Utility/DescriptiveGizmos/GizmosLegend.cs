using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.DescriptiveGizmos {
    public static class GizmosLegend {
        private static readonly Dictionary<MonoBehaviour, Dictionary<string, (Color, GizmoType)>> _legend = new ();

        public static IReadOnlyDictionary<MonoBehaviour, Dictionary<string, (Color, GizmoType)>> Legend => _legend;
        public static event Action OnLegendChanged = delegate { }; 

        public static void AddLabel(MonoBehaviour obj, string label, Color color, GizmoType gizmoType = GizmoType.None) {
            if (!_legend.ContainsKey(obj))
                _legend[obj] = new Dictionary<string, (Color, GizmoType)>();

            if (_legend[obj].TryGetValue(label, out (Color, GizmoType) value) && value.Item1 == color && value.Item2 == gizmoType)
                return;
            
            _legend[obj][label] = (color, gizmoType);
            OnLegendChanged.Invoke();
        }
        
        public static void RemoveLabel(MonoBehaviour obj, string label) {
            if (!_legend.TryGetValue(obj, out Dictionary<string, (Color, GizmoType)> value))
                return;

            if (!value.ContainsKey(label))
                return;

            value.Remove(label);
            OnLegendChanged.Invoke();
        }
        
        public static void Unregister(MonoBehaviour obj) {
            _legend.Remove(obj);
            OnLegendChanged.Invoke();
        }
    }
}