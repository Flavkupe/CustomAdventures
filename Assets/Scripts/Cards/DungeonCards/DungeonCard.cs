using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonCard<T> : Card<T>, IDungeonCard where T : DungeonCardData
{
    public override CardType CardType { get { return CardType.Dungeon; } }

    public abstract void ExecuteTileSpawnEvent(GridTile tile);
    public TDataType GetData<TDataType>() where TDataType : DungeonCardData
    {
        Debug.Assert(Data is TDataType, "Assumed incorrect data type for DungeonCard.");
        return Data as TDataType;
    }

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

    TDataType GetData<TDataType>() where TDataType : DungeonCardData;
}

public enum DungeonCardType
{
    Enemy,
    Treasure,
    Trap,

    Multi,
}

public enum DungeonEventType
{
    SpawnNear,
    SpawnOnCorner,
    SpawnOnWideOpen,

    MultiEvent,
}

public abstract class DungeonCardData : CardData
{
    public abstract DungeonCardType DungeonCardType { get; }
    public DungeonEventType DungeonEventType;
}