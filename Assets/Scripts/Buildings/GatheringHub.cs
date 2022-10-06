using System;
using System.Collections.Generic;
using Factory.Core;
using Factory.MapFeatures;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Buildings
{
    public class GatheringHub : ProductionBuilding, ISelectableBuilding, IDeliverable, IPickupable {
        [SerializeField] private ResourceNode node;
        [SerializeField] private Drone dronePrefab;
        

        /*protected override void Awake() {
            base.Awake();
        }*/

        private void Start() {
            droneLines.Add(new DroneLine(node, this, null));
            for (int i = 0; i < 1; i++) {
                var d = Instantiate(dronePrefab, transform.position + transform.right * .5f + Vector3.up * 0.25f, Quaternion.identity);
                droneLines[0].AddDrones(d);
            }
        }

        #region ISelectableBuilding
        public int UINum() => -1;

        public ProducableBuildings ProducableBuildingsType() => ProducableBuildings.gatherer;

        public void SetItem(Item item) { }
        #endregion

    }
}