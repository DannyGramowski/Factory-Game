using Factory.Buildings;
using Factory.Units.BaseUnits;

namespace Factory.Units.Actions {
    public class APickUp: IAction{
        private Drone _drone;
        private IPickupable _pickupable;

        public APickUp(Drone drone, IPickupable pickupable) {
            _drone = drone;
            _pickupable = pickupable;
        }

        public void OnInitiate() {
            _drone.PickUpItem(_pickupable.Pickup(_drone.GetItemType()));
        }

        public void OnTick() { }

        public bool IsFinished() => true;

        public string ActionName() => "ADropOff";

        public void OnExit() { }
    }
}