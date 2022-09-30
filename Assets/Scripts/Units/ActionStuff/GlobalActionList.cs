using System;
using System.Collections.Generic;
using System.IO;
using Factory.Core;
using UnityEngine;

namespace Factory.Units.Actions {
    public class GlobalActionList : Singleton<GlobalActionList> {
        public static readonly HashSet<string> ActionNames = new HashSet<string>();
        
        private void Awake() {
            const string filename = "/Scripts/Units/ActionStuff/Actions";
            var files = Directory.GetFiles(Application.dataPath + filename, "*.cs");
            foreach (var file in files) {
                var actionName = Path.GetFileName(file)[..^3];
                print("name is " + actionName);
                ActionNames.Add(actionName);
            }
        }
    }
}