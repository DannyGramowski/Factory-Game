namespace Factory.Buildings {
    public interface IDeliverable: IPosition {
        public void Deliver(ItemStack items, int amount);
    }
}