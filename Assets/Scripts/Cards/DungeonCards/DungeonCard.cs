using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonCard<T> : Card<T>, IDungeonCard where T : DungeonCardData
{
    public override CardType CardType { get { return CardType.Dungeon; } }
    public abstract DungeonCardType DungeonCardType { get; }    
    public abstract void ExecuteTileSpawnEvent(Tile tile);

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
    DungeonCardType DungeonCardType { get; }
    DungeonEventType DungeonEventType { get; }

    void ExecuteTileSpawnEvent(Tile tile);
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
}

public abstract class DungeonCardData : CardData
{
    public abstract DungeonCardType DungeonCardType { get; }
    public DungeonEventType DungeonEventType;
}