using UnityEditor;
using UnityEngine;

namespace Shooter.Agents.Editor {
    [CustomEditor(typeof(AgentManager))]
    public class AgentManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            if (!Application.isPlaying)
                return;
            
            var agentManager = (AgentManager)target;
            
            if (agentManager.IsGamePaused) {
                if (GUILayout.Button("Resume Game"))
                    agentManager.ResumeGame();
            } else {
                if (GUILayout.Button("Pause Game"))
                    agentManager.PauseGame();
            }
            
            if (GUILayout.Button("Tick"))
                AgentManager.Tick();
        }
    }
}