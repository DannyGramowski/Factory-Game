using Factory.Buildings;
using Factory.Core;

namespace Factory.Units.BaseUnits {
    public interface IHarvester {
        public void Harvest(Item item);
        public bool IsFinished();
    }
}