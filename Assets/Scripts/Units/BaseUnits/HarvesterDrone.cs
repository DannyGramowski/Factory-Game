using System.Collections.Generic;
using System.Drawing.Printing;
using Factory.Buildings;
using Factory.Units.Actions;
using System.Linq;
using Factory.Core;
using Factory.MapFeatures;
using UnityEngine;

namespace Factory.Units.BaseUnits {
    public class HarvesterDrone: Drone, IHarvester {
        private static Dictionary<string, IAction> _validHarvesterActions;
        [SerializeField] private ResourceNode node;

        new void Start() {
            if (_validHarvesterActions == null) {
                _validHarvesterActions = new Dictionary<string, IAction>();
                foreach (var action in GlobalActionList.Actions) {
                    if (actionName.Contains(action.ActionName())) {
                        _validHarvesterActions[action.ActionName()] = action;
                    }
                }
            }

            ExecuteAction(new AHarvestResource(),
                new object[][] { new object[] { node, this }, EmptyArgs, EmptyArgs });
        }
        
        public void Harvest(Item item) {
            Debug.Log("harvest");
            if(!ValidItem(item)) return;
            Debug.Log("valid item");
            if (_cargo.GetSize() >= stackCapacity) return;
            Debug.Log("added " + item);
            _cargo.AddItem(item);        
        }

        public bool IsFinished() => stackCapacity <= _cargo.GetSize();

        public override bool ValidAction(IAction action) => _validHarvesterActions.ContainsKey(action.ActionName());
        
    }
}