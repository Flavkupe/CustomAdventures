using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Tile entities that have stats and affected by StatusEffects.
/// </summary>
public abstract class TileActor : TileEntity, IDungeonActor
{
    public abstract Stats CurrentStats { get; }

    public List<StatusEffect> Effects { get; } = new List<StatusEffect>();

    public Transform Transform => this.transform;

    public virtual void AfterAppliedStatusEffect(StatusEffectData effect)
    {
    }

    public abstract void DoHealing(int healing);
}
