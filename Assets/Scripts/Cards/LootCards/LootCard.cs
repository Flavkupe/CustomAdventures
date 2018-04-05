using System.Collections.Generic;

public abstract class LootCard<T> : Card<T>, ILootCard where T : LootCardData
{
    public override CardType CardType { get { return CardType.Loot; } }

    public LootEventType LootEventType { get { return Data.LootEventType; } }

    public LootCardType LootCardType { get { return Data.LootCardType; } }

    public abstract void ExecuteLootGetEvent();
}

public interface ILootCard : ICard
{
    LootEventType LootEventType { get; }
    LootCardType LootCardType { get; }
    void ExecuteLootGetEvent();
}

public enum LootCardType
{
    Gold,
    Item,
    Weapon,
    Armor,
    Accessory,
}

public enum LootEventType
{
    GainLoot
}

public abstract class LootCardData : CardData
{
    public abstract LootCardType LootCardType { get; }
    public LootEventType LootEventType;
}

public class LootCardFilter
{
    public HashSet<LootCardType> PossibleTypes = new HashSet<LootCardType>();
}