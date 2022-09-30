using Factory.Buildings;
using Factory.Units.BaseUnits;

namespace Factory.Units.Actions {
    public class ADeliver : IAction {
        private Drone _drone;
        private IDeliverable _deliverable;


        public ADeliver(Drone drone, IDeliverable deliverable) {
            _drone = drone;
            _deliverable = deliverable;
        }
        
        public void OnInitiate() {
            _deliverable.Deliver(_drone.DeliverItem(_deliverable.GetMaxDeliverySize()));
        }

        public void OnTick() { }

        public bool IsFinished() => true;

        public string ActionName() => "ADeliver";

        public void OnExit() { }
    }
}