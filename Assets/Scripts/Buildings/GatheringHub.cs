using System.Collections.Generic;
using Factory.Core;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Buildings
{
    public class GatheringHub : ProductionBuilding, ISelectableBuilding, IDeliverable, IPickupable {
        private DroneLine line;
        [SerializeField] private List<Drone> drones;
        

        #region ISelectableBuilding
        public int UINum() => -1;

        public ProducableBuildings ProducableBuildingsType() => ProducableBuildings.gatherer;

        public void SetItem(Item item) { }
        #endregion

        #region deliverable and pickupable
        public void Deliver(Item[] items) {
            //throw new System.NotImplementedException();
        }

        public Item[] Pickup(Item itemtype) {
            return null;
            //throw new System.NotImplementedException();
        }

        Vector3 IPickupable.GetPosition() => transform.position;

        Vector3 IDeliverable.GetPosition() => transform.position;

        public int GetMaxDeliverySize() {
            return -1;
            //throw new System.NotImplementedException();
        }
        

        #endregion
        
    }
}