using Factory.Buildings;
using Factory.Core;

namespace Factory.Units.BaseUnits {
    public interface IItemCarrier {
        public int MaxStackSize();
        public bool ValidItem(Item item);
        public void PickUpItem(Item[] items);
        public Item[] DeliverItem(int maxSize);
    }
}