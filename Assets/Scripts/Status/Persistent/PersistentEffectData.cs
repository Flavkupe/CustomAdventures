using System.Collections;
using UnityEngine;

/// <summary>
/// A persistent effect is an effect like poison or buffs which sticks around and
/// eventually expires
/// </summary>
public abstract class PersistentEffectData : StatusEffectData
{
    [Tooltip("How long a status effect will last (based on the action provided)")]
    public int Duration;

    [Tooltip("Type of action that causes this effect to expire (such as steps)")]
    public EffectActivatorType ExpiryType;

    [Tooltip("Animation for when the effect expires")]
    public AnimationEffectData ExpireAnimationEffect;

    protected abstract PersistentStatusEffect CreateEffect();

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        yield return RunAnimationEffects(targetActor, source);
        targetActor.AddStatusEffect(CreateEffect());
        targetActor.AfterAppliedStatusEffect(this);
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

public abstract class PersistentStatusEffect
{
    public abstract void ActionTaken(TileActor subject, EffectActivatorType actionType);

    public abstract string GetIdentifier();

    public virtual void ModifyStats(Stats statsClone)
    {
    }

    public abstract void Expire(TileActor subject);
}

public abstract class PersistentStatusEffect<TDataType> : PersistentStatusEffect where TDataType : PersistentEffectData
{
    protected TDataType Data;

    /// <summary>
    /// A way to identify this effect data uniquely.
    /// </summary>
    public override string GetIdentifier()
    {
        return Data.GetIdentifier();
    }

    public override void Expire(TileActor subject)
    {
        // TODO: do we want to block on this?
        subject.StartCoroutine(EffectExpires(subject, this));
    }

    public virtual IEnumerator EffectExpires(TileActor targetActor, PersistentStatusEffect effect)
    {
        yield return RunExpireAnimationEffects(targetActor);
        targetActor.TryRemoveStatusEffect(effect);
    }

    protected virtual IEnumerator RunExpireAnimationEffects(TileActor targetActor)
    {
        if (Data.ExpireAnimationEffect != null)
        {
            var effect = Data.ExpireAnimationEffect.CreateEffect();
            effect.SetSourceEntity(targetActor);
            effect.SetTargetEntity(targetActor);
            yield return effect.CreateRoutine();
        }
    }
}
