using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Factory.Core;

namespace Factory.ItemEditor {
    [CreateAssetMenu(fileName = "new item hierarcy", menuName = "Item Hierarcy")]
    public class ItemHierarcy : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] List<ItemNode> nodes = new List<ItemNode>();

        Dictionary<string, ItemNode> nodeLookup = new Dictionary<string, ItemNode>();

#if UNITY_EDITOR
        private void Awake() {
            if (nodes.Count == 0) {
                CreateItemNodes();
               // CreateNode(null);
            }
        }
#endif

       

        private void OnValidate() {
            nodeLookup = new Dictionary<string, ItemNode>();
            foreach(ItemNode node in nodes) {
                if(node == null)  return;
                nodeLookup[node.guid] = node;
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

        public void SaveNodeChildren() {
            foreach(ItemNode itemNode in nodes) {
                Item item = itemNode.item;
                item.recipe.Clear();
                foreach(string id in itemNode.children) {
                    item.recipe.Add(nodeLookup[id].item);
                }
                PrefabUtility.SavePrefabAsset(item.gameObject);
            }
        }

        private void CreateItemNodes() {
            foreach(Item item in GlobalPointers.itemPrefabs) {
                CreateNode(item);
            }
        }

        public void CreateNode(Item item) {
            ItemNode node = CreateInstance<ItemNode>();
            node.guid = Guid.NewGuid().ToString();
            node.item = item;
            node.name = node.item.itemName;
            Undo.RegisterCreatedObjectUndo(node, "Created Dialogue Node");
          

            nodes.Add(node);
            Undo.RecordObject(this, "Remove Dialogue Link");
            OnValidate();
        }

       

        /*        public void CreateNode() {
                    DialogueNode rootNode = CreateInstance<DialogueNode>();
                    rootNode.guid = Guid.NewGuid().ToString();
                    nodes.Add(rootNode);
                }*/

        public void DeleteNode(ItemNode deleteNode) {
            Undo.RecordObject(this, "Remove Dialogue Link");
            RemoveFromChildren(deleteNode);
            nodes.Remove(deleteNode);
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
               // CreateNode(null);
            }

            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (ItemNode node in GetAllNodes()) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize() {

        }
    }
}

