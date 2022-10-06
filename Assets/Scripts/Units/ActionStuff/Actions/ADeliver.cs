using System;
using Factory.Buildings;
using Factory.Core;
using Factory.Units.BaseUnits;

namespace Factory.Units.Actions {
    [Serializable]
    public class ADeliver : IAction {
        private IItemCarrier _carrier;
        private IDeliverable _deliverable;
        private string _itemType;

        public ADeliver(IItemCarrier carrier, IDeliverable deliverable, string itemType) {
            _carrier = carrier;
            _deliverable = deliverable;
            _itemType = itemType;
        }
        
        public void OnInitiate() {
            _deliverable.Deliver(_carrier.GetDeliverStack(), _carrier.GetDeliverySize());
            //_deliverable.Deliver(_carrier.DeliverItem(_deliverable.GetMaxDeliverySize(_itemType)));
        }

        public void OnTick() { }

        public bool IsFinished() => true;

        public string ActionName() => "ADeliver";

        public void OnExit() { }
    }
}