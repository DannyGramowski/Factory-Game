using Factory.Core;

namespace Factory.Buildings {
    public interface ISelectableBuilding {
        public int UINum();//0:grabber / 1:miner / 2:assembler

        public ProducableBuildings ProducableBuildingsType();

        public void SetItem(Item item);
    }
}
