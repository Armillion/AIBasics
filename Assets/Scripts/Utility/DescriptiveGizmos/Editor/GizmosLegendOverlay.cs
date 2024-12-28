using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using GizmoType = Utility.GizmosLegend.GizmoType;

namespace Utility.DescriptiveGizmos.Editor {
    [Overlay(
        typeof(SceneView), "Gizmos Legend", true,
        minWidth = 150, minHeight = 20,
        defaultWidth = 150, defaultHeight = 150,
        maxWidth = int.MaxValue, maxHeight = int.MaxValue
    )]
    public class GizmosLegendOverlay : Overlay {
        private const float INDENT_SIZE = 12.5f;

        public override VisualElement CreatePanelContent() {
            var root = new ScrollView {
                name = "Root",
                style = {
                    paddingLeft = 5f,
                    paddingRight = 5f
                }
            };

            CreateLegend(root);

            GizmosLegend.GizmosLegend.OnLegendChanged += () => {
                root.Clear();
                CreateLegend(root);
            };

            return root;
        }

        private static void CreateLegend(VisualElement root) {
            var indentLevel = 0;

            foreach ((MonoBehaviour obj, Dictionary<string, (Color, GizmoType)> labels) in GizmosLegend.GizmosLegend.Legend) {
                Label objLabel = CreateObjectLabel(obj);
                root.Add(objLabel);
                indentLevel++;

                foreach ((string label, (Color color, GizmoType gizmoType)) in labels) {
                    VisualElement gizmoEntry = CreateGizmoEntry(label, color, indentLevel, gizmoType);
                    root.Add(gizmoEntry);
                }

                indentLevel--;
            }
        }

        private static VisualElement CreateGizmoEntry(string label, Color color, int indentLevel, GizmoType gizmoType) {
            var wrapper = new VisualElement {
                name = label,
                style = {
                    height = 16f,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                }
            };

            var labelElement = new Label(label) {
                style = {
                    color = color,
                    marginLeft = INDENT_SIZE * indentLevel
                }
            };

            wrapper.Add(labelElement);
                    
            var image = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Scripts/Utility/DescriptiveGizmos/Editor/Icons/{gizmoType}.png");

            if (!image)
                return wrapper;

            var gizmoTypeIcon = new Image {
                image = image,
                tintColor = color,
                style = { height = new StyleLength(Length.Auto()) },
                tooltip = gizmoType.ToString()
            };

            wrapper.Add(gizmoTypeIcon);
            return wrapper;
        }

        private static Label CreateObjectLabel(MonoBehaviour obj) {
            var objLabel = new Label(obj.name) {
                style = {
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    paddingLeft = 5f,
                    paddingRight = 5f,
                    paddingTop = 2.5f,
                    paddingBottom = 2.5f,
                    borderTopLeftRadius = 7.5f,
                    borderTopRightRadius = 7.5f,
                    borderBottomRightRadius = 7.5f,
                    borderBottomLeftRadius = 7.5f,
                }
            };

            objLabel.RegisterCallback<PointerUpEvent>(_ => {
                Selection.activeGameObject = obj.gameObject;
                EditorGUIUtility.PingObject(Selection.activeObject);
            });

            objLabel.RegisterCallback<MouseEnterEvent>(_ => {
                objLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            });

            objLabel.RegisterCallback<MouseLeaveEvent>(_ => {
                objLabel.style.backgroundColor = Color.clear;
            });

            return objLabel;
        }
    }
}