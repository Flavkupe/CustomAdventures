using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootCard<T> : Card<T>, ILootCard where T : LootCardData
{
    public override CardType CardType { get { return CardType.Loot; } }

    public LootEventType LootEventType { get { return Data.LootEventType; } }

    public abstract void ExecuteLootGetEvent();

    // Use this for initialization
    void Start ()
    {
	}

	// Update is called once per frame
	void Update ()
    {
	}
}

public interface ILootCard : ICard
{
    LootEventType LootEventType { get; }    
    void ExecuteLootGetEvent();
}

public enum LootCardType
{
    Gold,
    Item,
    Weapon,
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