using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Tile entities that have stats and affected by StatusEffects.
/// </summary>
public abstract class TileActor : TileEntity, IDungeonActor
{
    protected ThoughtBubble ThoughtBubble { get; private set; }

    public abstract Stats CurrentStats { get; }

    public List<StatusEffect> Effects { get; } = new List<StatusEffect>();

    public Transform Transform => this.transform;

    public virtual void AfterAppliedStatusEffect(StatusEffectData effect)
    {
    }

    public abstract void DoHealing(int healing);

    protected virtual void Init()
    {
        // Populate all Entity parts
        ThoughtBubble = Instantiate(Game.Prefabs.EntityParts.ThoughtBubble);
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