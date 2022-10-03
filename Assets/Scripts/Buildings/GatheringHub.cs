using System.Collections.Generic;
using Factory.Core;
using Factory.MapFeatures;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Buildings
{
    public class GatheringHub : ProductionBuilding, ISelectableBuilding, IDeliverable, IPickupable {
        [SerializeField] private List<Drone> drones;
        [SerializeField] private ResourceNode node;
        

        protected override void Awake() {
            base.Awake();
            var droneLine = new DroneLine(node, this, null);
            base.AddDroneLine(droneLine);
        }

        #region ISelectableBuilding
        public int UINum() => -1;

        public ProducableBuildings ProducableBuildingsType() => ProducableBuildings.gatherer;

        public void SetItem(Item item) { }
        #endregion

    }
}