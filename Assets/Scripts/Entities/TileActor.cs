using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class for Tile entities that have stats and affected by StatusEffects.
/// </summary>
public abstract class TileActor : TileEntity, IDungeonActor
{
    protected ThoughtBubble ThoughtBubble { get; private set; }

    public abstract Stats CurrentStats { get; }
    public abstract Stats BaseStats { get; }

    public List<PersistentStatusEffect> Effects { get; } = new List<PersistentStatusEffect>();

    public Transform Transform => this.transform;

    public virtual void AfterAppliedStatusEffect(StatusEffectData effect)
    {
    }

    public void AddStatusEffect(PersistentStatusEffect effect)
    {
        Effects.Add(effect);
    }

    public void TryRemoveStatusEffect(PersistentStatusEffect effect)
    {
        if (effect != null && Effects.Contains(effect))
        {
            Effects.Remove(effect);
        }
    }

    /// <summary>
    /// Gets the stats modified by each Effect. Creates
    /// a clone of the stats, does not modify in place.
    /// </summary>
    /// <param name="baseStats">Whether this is the modified base stats or current stats</param>
    /// <returns></returns>
    public Stats GetModifiedStats(bool baseStats = false)
    {
        var statsClone = baseStats ? BaseStats.Clone() : CurrentStats.Clone();
        foreach (var effect in Effects)
        {
            effect.ModifyStats(statsClone);
        }

        return statsClone;
    }

    public abstract void DoHealing(int healing);

    protected virtual void Init()
    {
        // Populate all Entity parts
        ThoughtBubble = Instantiate(Game.Dungeon.Templates.EntityParts.ThoughtBubble);
        ThoughtBubble.transform.SetParent(transform, false);
        ThoughtBubble.gameObject.SetActive(false);
    }

    public void HideThoughtBubble()
    {
        ThoughtBubble.gameObject.SetActive(false);
    }

    public void SetThoughtBubbleText(string text)
    {
        ThoughtBubble.gameObject.SetActive(true);
        ThoughtBubble.SetText(text);
    }
}

public abstract class TileAI : TileActor, IAIDungeonActor
{
    public abstract BehaviorList Behavior { get; }

    public abstract IEnumerator ProcessCharacterTurn();
}