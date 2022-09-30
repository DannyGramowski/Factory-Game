using System.Collections.Generic;
using System.Drawing.Printing;
using Factory.MapFeatures;
using Factory.Units.Actions;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Buildings {
    public class DroneLine {
        private List<Drone> _drone = new List<Drone>();
        private IDeliverable _deliveryPoint;
        private IPickupable _pickupPoint;

        public DroneLine(IPickupable pickupPoint, IDeliverable deliveryPoint) {
            _pickupPoint = pickupPoint;
            _deliveryPoint = deliveryPoint;
        }

        private void SetDroneActions(Drone drone) {
            IAction[] actionSet = {
                new ADroneMove(drone, _pickupPoint.GetPosition()),
                null, //will set later
                new ADroneMove(drone, _deliveryPoint.GetPosition()),
                new ADeliver(drone, _deliveryPoint)
            };
            
            IAction pickupAction = _pickupPoint is ResourceNode
                ? new AHarvestResource((ResourceNode)_pickupPoint, drone as IHarvester)
                : new APickUp(drone, _pickupPoint);
            actionSet[1] = pickupAction;
            
            drone.SetActionSet(new ActionSet(drone,actionSet,looping:true));
        }
    }
}