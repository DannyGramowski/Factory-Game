using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Factory.ItemEditor.Editor {
    public class ItemHierarcyEditor : EditorWindow {
        ItemHierarcy selectedHierarcy = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] ItemNode draggingNode = null;
        [NonSerialized] ItemNode creatingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] ItemNode deletingNode = null;
        [NonSerialized] ItemNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 50f;

        [MenuItem("Window/Item Hierarchy Editor")]
        public static void ShowEditorWindow() {
            GetWindow(typeof(ItemHierarcyEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            ItemHierarcy dialogue = EditorUtility.InstanceIDToObject(instanceID) as ItemHierarcy;
            if (dialogue != null) {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange() {
            ItemHierarcy newDialogue = Selection.activeObject as ItemHierarcy;
            if (newDialogue != null) {
                selectedHierarcy = newDialogue;
                Repaint();
            }

        }

        private void OnGUI() {
            if (selectedHierarcy == null) {
                EditorGUILayout.LabelField("no dialogue selected");
            } else {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                //Texture2D backgroundText = Resources.Load("background") as Texture2D;
               // Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
              //  GUI.DrawTextureWithTexCoords(canvas, backgroundText, texCoords);
                foreach (ItemNode node in selectedHierarcy.GetAllNodes()) {
                    DrawNode(node);
                    DrawConnections(node);
                }
                EditorGUILayout.EndScrollView();
                if (creatingNode != null) {
                    Undo.RecordObject(selectedHierarcy, "Added Dialogue Node");
                    
                    selectedHierarcy.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (creatingNode != null) {
                    Undo.RecordObject(selectedHierarcy, "Added Dialogue Node");
                    selectedHierarcy.DeleteNode(creatingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents() {
            Debug.Log("dragging node is " + draggingNode);
            if (Event.current.type == EventType.MouseDown && draggingNode == null) {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null) {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                } else {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                }
            } else if (Event.current.type == EventType.MouseDrag && draggingNode != null) {
                Debug.Log("moving dragging node");
                Undo.RecordObject(selectedHierarcy, "move dialouge node");
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
        /*        Rect temp = draggingNode.rect;
                temp.position = ;*/
                GUI.changed = true;
            } else if (Event.current.type == EventType.MouseDrag && draggingCanvas) {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            } else if (Event.current.type == EventType.MouseUp && draggingNode != null) {
                draggingNode = null;
            } else if (Event.current.type == EventType.MouseUp && draggingCanvas) {
                draggingCanvas = false;
            }
        }


        private void DrawNode(ItemNode node) {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();


            EditorGUILayout.LabelField("Node: ", EditorStyles.whiteLabel);
            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck()) {

                Undo.RecordObject(node, "Update dialogue Text");
                node.text = newText;
            }

            foreach (ItemNode childNode in selectedHierarcy.GetAllChildren(node)) {
                EditorGUILayout.LabelField(childNode.text);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                creatingNode = node;
            }

            DrawLinkButtones(node);

            if (GUILayout.Button("x")) {
                deletingNode = node;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawLinkButtones(ItemNode node) {
            if (linkingParentNode == null) {
                if (GUILayout.Button("link")) {
                    linkingParentNode = node;
                }

            } else if (linkingParentNode == node) {
                if (GUILayout.Button("cancel")) {
                    linkingParentNode = null;
                }

            } else if (linkingParentNode.children.Contains(node.guid)) {
                if (GUILayout.Button("unlink")) {
                    linkingParentNode.children.Remove(node.guid);
                    linkingParentNode = null;
                }
            } else {
                if (GUILayout.Button("child")) {
                    linkingParentNode.children.Add(node.guid);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(ItemNode node) {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (ItemNode childNode in selectedHierarcy.GetAllChildren(node)) {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        private ItemNode GetNodeAtPoint(Vector2 point) {
            ItemNode foundNode = null;
            foreach (ItemNode node in selectedHierarcy.GetAllNodes()) {
                if (node.rect.Contains(point)) {
                    foundNode = node;
                }
            }
            Debug.Log("get node at point returned " + foundNode);
            return foundNode;
        }

    }
}