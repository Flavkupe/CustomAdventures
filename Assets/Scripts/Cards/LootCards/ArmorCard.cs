public class ArmorCard : ItemCard<ArmorCardData> {
    protected override InventoryItem CreateBackingItem()
    {
        return new Armor(Data);
    }
}
