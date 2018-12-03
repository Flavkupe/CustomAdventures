using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// These are persistent effects which proc an effect when actions happen, such as poison or regen.
/// </summary>
[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Enchantment Effect", order = 1)]
public class EnchantmentEffectData : PersistentEffectData
{
    public Stats StatusChange;   

    protected override PersistentStatusEffect CreateEffect()
    {
        return new EnchantmentEffect(this);
    }

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        yield return RunAnimationEffects(targetActor, source);
        targetActor.AddStatusEffect(CreateEffect());
        targetActor.AfterAppliedStatusEffect(this);
    }
}

public class EnchantmentEffect : PersistentStatusEffect<EnchantmentEffectData>
{
    public EnchantmentEffect(EnchantmentEffectData data)
        : base(data)
    {
        Data = data;
        _duration = data.Duration;
    }

    private int _duration;

    public override void ActionTaken(TileActor subject, EffectActivatorType actionType)
    {
        if (Data.ExpiryType == EffectActivatorType.Any || actionType == Data.ExpiryType)
        {
            _duration--;
            if (_duration <= 0)
            {
                Expire(subject);
            }
        }
    }

    /// <summary>
    /// Modifies the passed-in stats in place, according to the effect of the enchantment
    /// </summary>
    public override void ModifyStats(Stats stats)
    {
        stats.Accumulate(Data.StatusChange);
    }
}
