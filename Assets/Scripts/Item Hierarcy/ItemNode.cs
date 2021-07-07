using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Factory.ItemEditor {
    public class ItemNode : ScriptableObject {
        public string guid;
        public string text {
            get => _text;
            set => SetText(value);
        }
        public List<string> children { get => _children; }
        public Rect rect { get => _rect; }

        private string _text;
        private List<string> _children = new List<string>();
        private Rect _rect = new Rect(0, 0, 200, 100);

#if UNITY_EDITOR
        void SetText(string text) {
            if (text != _text) {
                Undo.RecordObject(this, "update dialogue text");
                _text = text;
            }
        }

        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "update dialogue text");
            _rect.position = newPosition;
        }

        public void AddChild(string childID) {
            Undo.RecordObject(this, "add dialogue link");
            children.Add(childID);
        }

        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "remove dialogue link");
            children.Remove(childID);
        }
    }
#endif

}
