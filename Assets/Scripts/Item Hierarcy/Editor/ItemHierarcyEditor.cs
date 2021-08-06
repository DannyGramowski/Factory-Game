using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Factory.Core;

namespace Factory.ItemEditor.Editor {
    public class ItemHierarcyEditor : EditorWindow {
        ItemHierarcy selectedHierarcy = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] ItemNode draggingNode = null;
        [NonSerialized] ItemNode creatingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] ItemNode deletingNode = null;
        [NonSerialized] ItemNode linkingParentNode = null;
        [NonSerialized] ItemNode unLinkingParentNode = null;
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
                CreateToolbar();
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
                    
                 //   selectedHierarcy.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null) {
                   // Undo.RecordObject(selectedHierarcy, "Added Dialogue Node");
                    selectedHierarcy.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void CreateToolbar() {
            GUILayout.BeginArea(new Rect(0,0,225,25));
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Create Item")) {
                OnCreate onCreate = OnCreateItem;
                CloseWindow closeWindow = OnCloseWindow;
                CreateItemWindow.Open(closeWindow,onCreate);
            }
            if(GUILayout.Button("Save children")) {
                selectedHierarcy.SaveNodeChildren();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void OnCreateItem(Item newItem) {
            Debug.Log("on create item with item of " + newItem);
            selectedHierarcy.CreateNode(newItem);
        }

        void OnCloseWindow() {

        }

        private void ProcessEvents() {
           // Debug.Log(Event.current.type);
            if (Event.current.type == EventType.MouseDown) {
                ItemNode selectedNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
        
                if (linkingParentNode != null) {
                    if(selectedNode != null) linkingParentNode.AddChild(selectedNode.guid);
                    if(!Input.GetKey(KeyCode.LeftShift)) {
                        linkingParentNode = null;
                        
                    }
                } else if (unLinkingParentNode != null) {
                    if(selectedNode != null) unLinkingParentNode.RemoveChild(selectedNode.guid);
                    if (!Input.GetKey(KeyCode.LeftShift)) {
                        unLinkingParentNode = null;
                    }
                } else if(selectedNode != null){

                    
                    draggingNode = selectedNode;
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode.item;
                }else {
                draggingCanvas = true;
                draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                }
            } else if (Event.current.type == EventType.MouseDrag) {
                if(draggingNode != null) {
                    Undo.RecordObject(selectedHierarcy, "move dialouge node");
                    draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                } else if(draggingCanvas) {
                    scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                } 
                GUI.changed = true;
            } else if (Event.current.type == EventType.MouseUp) {
                /*if(linkingParentNode != null) {
                    Debug.Log("Mouse down set Linking node");
                    ItemNode childNode = GetNodeAtPoint(Input.mousePosition);
                    Debug.Log("child node is " + childNode);
                    if(childNode != null) {
                        linkingParentNode.AddChild(childNode.guid);
                        Debug.Log("added child");
                    }
                    linkingParentNode = null;
                } else if(unLinkingParentNode != null) {
                    ItemNode childNode = GetNodeAtPoint(Input.mousePosition);
                    if (childNode != null) {
                        unLinkingParentNode.children.Remove(childNode.guid);
                    }
                    unLinkingParentNode = null;
                }*/
                if(draggingNode != null) {
                    draggingNode = null;
                } else if(draggingCanvas) {
                    draggingCanvas = false;
                }  
            }
        }


        private void DrawNode(ItemNode node) {
            GUILayout.BeginArea(node.rect, nodeStyle);
            // EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
                DrawSprite(node, node.rect);
                EditorGUILayout.LabelField(node.item.itemName, EditorStyles.whiteLabel);
                EditorGUILayout.LabelField(node.item.producableBuilding.ToString(), EditorStyles.whiteLabel);
            EditorGUILayout.EndVertical();

           /* if (EditorGUI.EndChangeCheck()) {

                Undo.RecordObject(node, "Update dialogue Text");
               // node.text = newText;
            }*/

/*            foreach (ItemNode childNode in selectedHierarcy.GetAllChildren(node)) {
                EditorGUILayout.LabelField(childNode.text);
            }*/

             GUILayout.BeginHorizontal();
           if (GUILayout.Button("x")) {
                deletingNode = node;
            }

            DrawLinkButtones(node);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawLinkButtones(ItemNode node) {
            if (GUILayout.Button("link")) {
                linkingParentNode = node;
                Debug.Log("set linking parent node");
            }

            if (node.children.Count != 0) {
                if (GUILayout.Button("unlink")) {
                    unLinkingParentNode = node;
                    Debug.Log("set unlinking parent node");

                }
            }
        }

        private void DrawConnections(ItemNode node) {
            Vector3 startPosition = new Vector2(node.rect.xMin, node.rect.center.y);
            foreach (ItemNode childNode in selectedHierarcy.GetAllChildren(node)) {
                Vector3 endPosition = new Vector2(childNode.rect.xMax, childNode.rect.center.y);
                DrawConnection(startPosition, endPosition);
            }
        }

        void DrawConnection(Vector3 startPosition, Vector3 endPosition) {
            Vector3 controlPointOffset = endPosition - startPosition;
            controlPointOffset.y = 0;
            controlPointOffset *= 0.8f;
            Handles.DrawBezier(startPosition, endPosition,
                startPosition + controlPointOffset,
                endPosition - controlPointOffset,
                Color.white, null, 4f);
        }

        void DrawSprite(ItemNode node, Rect areaRect) {
            Sprite itemSprite = node.item.sprite;
            Rect itemRect = itemSprite.rect;
            float scaleFactor = (areaRect.width - 50) / itemRect.width;
            float spriteW = itemRect.width * scaleFactor;
            float spriteH = itemRect.height * scaleFactor;
            Rect c = GUILayoutUtility.GetRect(spriteW,spriteH);
            c.x = 20;
            c.y = 20;
            
           // Rect rect = GUILayoutUtility.GetRect(spriteW, spriteH);
            if (Event.current.type == EventType.Repaint) {
                var tex = itemSprite.texture;

                GUI.DrawTextureWithTexCoords(c, tex, new Rect(0, 0, 1, 1));
            }
        }
        private ItemNode GetNodeAtPoint(Vector2 point) {
            ItemNode foundNode = null;
            foreach (ItemNode node in selectedHierarcy.GetAllNodes()) {
                if (node.rect.Contains(point)) {
                    foundNode = node;
                }
            }
            return foundNode;
        }

    }
}