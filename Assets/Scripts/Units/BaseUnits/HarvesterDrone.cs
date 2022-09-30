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
        [SerializeField] private ResourceNode node;

        public void Harvest(Item item) {
            if(!ValidItem(item)) return;
            if (_cargo.GetSize() >= stackCapacity) return;
            _cargo.AddItem(item);        
        }

        public bool IsFinished() => stackCapacity <= _cargo.GetSize();

        
    }
}