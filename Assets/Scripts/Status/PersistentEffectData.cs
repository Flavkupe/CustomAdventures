using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// A persistent effect is an effect like poison which keeps triggering after an action
/// </summary>
[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Persistent Effect", order = 1)]
public class PersistentEffectData : StatusEffectData
{
    [Tooltip("A Status effect that will happen after each action")]
    public StatusEffectData StatusEffectData;

    [Tooltip("How long a status effect will last (based on the action provided)")]
    public int Duration;

    [Tooltip("Type of action that causes this effect to expire (such as steps)")]
    public EffectActivatorType ExpiryType;

    [Tooltip("Type of action that causes this effect apply")]
    public EffectActivatorType ProcType;

    [Tooltip("Animation for when the effect expires")]
    public AnimationEffectData ExpireAnimationEffect;

    public StatusEffect CreateEffect()
    {
        return new StatusEffect(this);
    }

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        yield return RunAnimationEffects(targetActor, source);
        targetActor.AddPersistentEffect(CreateEffect());
        targetActor.AfterAppliedStatusEffect(this);
    }

    public virtual IEnumerator EffectExpires(TileActor targetActor, StatusEffect effect)
    {
        yield return RunExpireAnimationEffects(targetActor);
        targetActor.RemovePersistentEffect(effect);
    }

    public virtual IEnumerator ProcEffectOn(TileActor targetActor)
    {
        yield return StatusEffectData.ApplyEffectOn(targetActor, targetActor);
    }

    protected virtual IEnumerator RunExpireAnimationEffects(TileActor targetActor)
    {
        if (ExpireAnimationEffect != null)
        {
            var effect = ExpireAnimationEffect.CreateEffect();
            effect.SetSourceEntity(targetActor);
            effect.SetTargetEntity(targetActor);
            yield return effect.CreateRoutine();
        }
    }
}

/// <summary>
/// Actions that activate or expire this effect
/// </summary>
public enum EffectActivatorType
{
    None,
    Any,
    Turns,
    Attacks,
    Steps,
}

[Serializable]
public class StatusEffect
{
    public StatusEffect(PersistentEffectData data)
    {
        Data = data;
        _duration = data.Duration;
    }

    protected PersistentEffectData Data;

    private int _duration;

    public virtual void Proc(TileActor subject)
    {
        // TODO: do we want to block on this?
        subject.StartCoroutine(Data.ProcEffectOn(subject));
    }

    public virtual void Expire(TileActor subject)
    {
        // TODO: do we want to block on this?
        subject.StartCoroutine(Data.EffectExpires(subject, this));
    }

    public void ActionTaken(TileActor subject, EffectActivatorType actionType)
    {
        if (Data.ProcType == EffectActivatorType.Any || actionType == Data.ProcType)
        {
            Proc(subject);
        }

        if (Data.ExpiryType == EffectActivatorType.Any || actionType == Data.ExpiryType)
        {
            _duration--;
            if (_duration <= 0)
            {
                Expire(subject);
            }
        }
    }
}
