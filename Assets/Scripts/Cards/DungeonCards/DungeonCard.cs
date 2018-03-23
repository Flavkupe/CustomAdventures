using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonCard<T> : Card<T>, IDungeonCard where T : DungeonCardData
{
    public override CardType CardType { get { return CardType.Dungeon; } }

    public abstract void ExecuteTileSpawnEvent(GridTile tile);

    public DungeonEventType DungeonEventType { get { return Data.DungeonEventType; } }

    // Use this for initialization
    void Start ()
    {
	}

	// Update is called once per frame
	void Update ()
    {
	}
}

public interface IDungeonCard : ICard
{
    DungeonEventType DungeonEventType { get; }

    void ExecuteTileSpawnEvent(GridTile tile);
}

public enum DungeonCardType
{
    Enemy,
    Treasure,
    Trap,
}

public enum DungeonEventType
{
    SpawnNear,
    SpawnOnCorner,
    SpawnOnWideOpen,
}

public abstract class DungeonCardData : CardData
{
    public abstract DungeonCardType DungeonCardType { get; }
    public DungeonEventType DungeonEventType;
}