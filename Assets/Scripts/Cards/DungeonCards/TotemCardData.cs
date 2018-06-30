using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Totem Card", order = 1)]
public class TotemCardData : EntityCardData<CurseTotem>
{
    public Sprite Sprite;

    public int Level = 1;

    public PersistentEffectData CurseEffect;

    public override DungeonCardType DungeonCardType => DungeonCardType.Totem;

    public override Type BackingCardType => typeof(TotemCard);

    public AudioClip[] BreakSounds;

    public override CurseTotem InstantiateEntity()
    {
        var entity = Utils.InstantiateOfType<CurseTotem>();
        entity.Data = this;
        return entity;
    }
}
