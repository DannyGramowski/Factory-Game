using Factory.Buildings;
using Factory.Core;
using Factory.Units.BaseUnits;

namespace Factory.Units.Actions {
    public class ADeliver : IAction {
        private IItemCarrier _carrier;
        private IDeliverable _deliverable;
        private Item _itemType;

        public ADeliver(IItemCarrier carrier, IDeliverable deliverable, Item item) {
            _carrier = carrier;
            _deliverable = deliverable;
            _itemType = item;
        }
        
        public void OnInitiate() {
            _deliverable.Deliver(_carrier.DeliverItem(_deliverable.GetMaxDeliverySize(_itemType)));
        }

        public void OnTick() { }

        public bool IsFinished() => true;

        public string ActionName() => "ADeliver";

        public void OnExit() { }
    }
}