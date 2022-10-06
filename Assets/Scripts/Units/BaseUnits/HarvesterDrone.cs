using Factory.Core;

namespace Factory.Units.BaseUnits {
    public class HarvesterDrone: Drone, IHarvester {

        public void Harvest(Item item) {
            if(!ValidItem(item.itemName)) return;
            if (_cargo.GetSize() >= stackCapacity) return;
            _cargo.AddItem(item);        
        }

        public bool IsFinished() => stackCapacity <= _cargo.GetSize();

        
    }
}