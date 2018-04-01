using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/MultiEvent Card", order = 1)]
public class MultiEventCardData : DungeonCardData
{
    public DungeonCardData[] Events;

    public bool Boss = false;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Multi; } }

    public override Type BackingCardType { get { return typeof(MultiEventCard); } }

    [Tooltip("Only used with PickXRandomly MultiEventType")]
    public int NumberOfEvents = 2;

    public MultiEventType MultiEventType;
}

public enum MultiEventType
{
    DoEach,
    PickXRandomly,
}
