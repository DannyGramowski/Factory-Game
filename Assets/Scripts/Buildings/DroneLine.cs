using System.Collections.Generic;
using System.Drawing.Printing;
using Factory.Core;
using Factory.MapFeatures;
using Factory.Units.Actions;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Buildings {
    public class DroneLine {
        private List<Drone> _drones = new List<Drone>();
        private IDeliverable _deliveryPoint;
        private IPickupable _pickupPoint;
        private Item _itemType;

        public DroneLine(IPickupable pickupPoint, IDeliverable deliveryPoint, Item itemType) {
            _pickupPoint = pickupPoint;
            _deliveryPoint = deliveryPoint;
            _itemType = itemType;
        }

        public void AddDrones(List<Drone> drones) {
            foreach (var drone in drones) {
                SetDroneActions(drone);
                drones.Add(drone);
            }    
        }
        
        private void SetDroneActions(Drone drone) {
            IAction[] actionSet = {
                new ADroneMove(drone, _pickupPoint.GetPosition()),
                null, //will set later
                new ADroneMove(drone, _deliveryPoint.GetPosition()),
                new ADeliver(drone, _deliveryPoint, drone.GetItemType())
            };
            
            IAction pickupAction = _pickupPoint is ResourceNode
                ? new AHarvestResource((ResourceNode)_pickupPoint, drone as IHarvester)
                : new APickUp(drone, _pickupPoint, _itemType);
            actionSet[1] = pickupAction;
            
            drone.SetActionSet(new ActionSet(drone,actionSet,looping:true));
        }
    }
}