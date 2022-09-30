using Factory.Buildings;
using Factory.Core;

namespace Factory.Units.BaseUnits {
    public interface IItemCarrier {
        public int MaxStackSize();
        public bool ValidItem(Item item);
        public void PickUpItem(ItemStack itemStack);
        public Item[] DeliverItem(int maxSize);
    }
}