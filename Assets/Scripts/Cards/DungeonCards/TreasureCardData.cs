using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Treasure Card", order = 1)]
public class TreasureCardData : EntityCardData<Treasure>
{
    public Sprite Sprite;

    public int Level = 1;

    public int NumTreasures = 1;

    public LootCardType[] LootTypes;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Treasure; } }
    public override Type BackingCardType { get { return typeof(TreasureCard); } }

    public AudioClip[] OpenSounds;

    public override Treasure InstantiateEntity()
    {
        var entity = Utils.InstantiateOfType<Treasure>();
        entity.Data = this;
        return entity;
    }
}
