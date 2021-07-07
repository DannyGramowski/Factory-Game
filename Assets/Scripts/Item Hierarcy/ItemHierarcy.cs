using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Factory.ItemEditor {
    [CreateAssetMenu(fileName = "new item hierarcy", menuName = "Item Hierarcy")]
    public class ItemHierarcy : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] List<ItemNode> nodes = new List<ItemNode>();

        Dictionary<string, ItemNode> nodeLookup = new Dictionary<string, ItemNode>();

#if UNITY_EDITOR
        private void Awake() {
            if (nodes.Count == 0) {
                CreateNode(null);
            }
        }
#endif

        private void OnValidate() {
            Debug.Log("on Validate");
        
            nodeLookup = new Dictionary<string, ItemNode>();
            foreach(ItemNode node in nodes) {
                Debug.Log("on validate node " + node);
                if(node == null) {
                    Debug.Log("null node");
                    return;
                }
                nodeLookup
                    [node.guid] =
                    node;
            }
        }
        public IEnumerable<ItemNode> GetAllNodes() {
            return nodes;
        }

        public ItemNode GetRootNode() {
            return nodes[0];
        }

        public IEnumerable<ItemNode> GetAllChildren(ItemNode parentNode) {
            //List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childID in parentNode.children) {
                if (nodeLookup.ContainsKey(childID)) yield return nodeLookup[childID];
            }

        }

        public void CreateNode(ItemNode parent) {
        Debug.Log("create node");
            ItemNode node = CreateInstance<ItemNode>();
            node.guid = Guid.NewGuid().ToString();
            Debug.Log("node " + node + " with type " + node.GetType());
            Undo.RegisterCreatedObjectUndo(node, "Created Dialogue Node");
            if (parent != null) {
                parent.children.Add(node.guid);
            }

            nodes.Add(node);
            Undo.RecordObject(this, "Remove Dialogue Link");
            OnValidate();

            foreach(ItemNode temp in nodes) {
                Debug.Log(temp + " in nodes");
            }
        }

        /*        public void CreateNode() {
                    DialogueNode rootNode = CreateInstance<DialogueNode>();
                    rootNode.guid = Guid.NewGuid().ToString();
                    nodes.Add(rootNode);
                }*/

        public void DeleteNode(ItemNode deleteNode) {
            Undo.RecordObject(this, "Remove Dialogue Link");
            nodes.Remove(deleteNode);
            RemoveFromChildren(deleteNode);
            OnValidate();
            Undo.DestroyObjectImmediate(deleteNode);

        }

        private void RemoveFromChildren(ItemNode deleteNode) {
            foreach (ItemNode node in GetAllNodes()) {
                node.children.Remove(deleteNode.guid);
            }
        }

        public void OnBeforeSerialize() {
            if (nodes.Count == 0) {
                CreateNode(null);
            }

            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (ItemNode node in GetAllNodes()) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        Debug.Log("added node to " + name);
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize() {

        }
    }
}

