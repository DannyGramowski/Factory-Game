using Factory.Buildings;
using Factory.Core;
using Factory.Units.BaseUnits;

namespace Factory.Units.Actions {
    public class APickUp: IAction{
        private IItemCarrier _carrier;
        private IPickupable _pickupable;
        private Item _item;
        public APickUp(IItemCarrier carrier, IPickupable pickupable, Item itemType) {
            _carrier = carrier;
            _pickupable = pickupable;
            _item = itemType;
        }

        public void OnInitiate() {
            _carrier.PickUpItem(_pickupable.Pickup(_item, _carrier.MaxStackSize()));
        }

        public void OnTick() { }

        public bool IsFinished() => true;

        public string ActionName() => "ADropOff";

        public void OnExit() { }
    }
}