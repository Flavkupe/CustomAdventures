public class WeaponCard : ItemCard<WeaponCardData> {

    protected override InventoryItem CreateBackingItem()
    {
        return new Weapon(Data);
    }
}
