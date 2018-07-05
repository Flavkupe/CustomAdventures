using System.Collections;
using System.Collections.Generic;

public abstract class LootCard<T> : Card<T>, ILootCard where T : LootCardData
{
    public override CardType CardType { get { return CardType.Loot; } }

    public LootEventType LootEventType { get { return Data.LootEventType; } }

    public LootCardType LootCardType { get { return Data.LootCardType; } }

    protected abstract IEnumerator ExecuteGetLootEvent(LootCardExecutionContext context);

    public IEnumerator ExecuteLootEvent(LootCardExecutionContext context)
    {
        var defaultAnimation = GetDefaultCardTriggerEffect();
        yield return AnimateCardTriggerEffect(defaultAnimation);
        yield return ExecuteGetLootEvent(context);
    }
}

public interface ILootCard : ICard
{
    LootEventType LootEventType { get; }
    LootCardType LootCardType { get; }
    IEnumerator ExecuteLootEvent(LootCardExecutionContext context);
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

public class LootCardExecutionContext
{
    public Player Player { get; private set; }

    public LootCardExecutionContext(Player player)
    {
        Player = player;
    }
}