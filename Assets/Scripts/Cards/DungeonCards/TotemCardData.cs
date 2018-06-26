using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Totem Card", order = 1)]
public class TotemCardData : EntityCardData<CurseTotem>
{
    public Sprite Sprite;

    public int Level = 1;

    public int NumTreasures = 1;

    public LootCardType[] LootTypes;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Treasure; } }
    public override Type BackingCardType { get { return typeof(TreasureCard); } }

    public AudioClip[] OpenSounds;

    public override CurseTotem InstantiateEntity()
    {
        var entity = Utils.InstantiateOfType<CurseTotem>();
        entity.Data = this;
        return entity;
    }
}
