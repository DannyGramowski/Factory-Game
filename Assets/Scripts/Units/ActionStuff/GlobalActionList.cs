using System;
using System.Collections.Generic;
using System.IO;
using Factory.Core;
using UnityEngine;

namespace Factory.Units.Actions {
    public class GlobalActionList : Singleton<GlobalActionList> {
        public static readonly List<IAction> Actions = new List<IAction>();

        
        private void Awake() {
            Actions.Add(new ADroneMove());
            Actions.Add(new AHarvestResource());
            
            const string filename = "/Scripts/Units/ActionStuff/Actions";
            var actionsFileCount = Directory.GetFiles(Application.dataPath + filename, "*.cs").Length;
            Debug.Assert(Actions.Count == actionsFileCount, "actions loaded and actions available dont match");
        }
    }
}