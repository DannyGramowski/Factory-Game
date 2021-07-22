using Factory.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Factory.ItemEditor {
    public class ItemNode : ScriptableObject {
        public string guid;
        public List<string> children { get => _children; }
        public Rect rect { get => _rect; }
        public Item item;

        private List<string> _children = new List<string>();
        private Rect _rect = new Rect(0, 0, 125, 175);

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "update dialogue text");
            _rect.position = newPosition;
        }

        public void AddChild(string childID) {
            Undo.RecordObject(this, "add dialogue link");
            children.Add(childID);
        }
        public void RemoveChildren() {
            foreach(string id in children) {
                RemoveChild(id);
            }
        }
        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "remove dialogue link");
            children.Remove(childID);
        }
    }
#endif

}
