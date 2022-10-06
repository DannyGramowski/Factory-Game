namespace Factory.Buildings {
    public interface IPickupable: IPosition {
        
        public void Pickup(ItemStack itemStack, int amount);
    }
}