using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Instant Effect", order = 1)]
public class InstantEffectData : StatusEffectData
{
    [Tooltip("How strong the effect is, ie HP gained, damage taken per turn, etc")]
    public int Magnitude;

    public InstantEffectType EffectType;
    public ExtraEffectProperties ExtraProperties;

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        yield return ActivateEffect(targetActor, source, Magnitude);
    }

    public override IEnumerator ApplyEffectOn(TileActor targetActor, TileActor source)
    {
        var magnitude = Magnitude;
        if (ExtraProperties.HasFlag(ExtraEffectProperties.BoostedByStrength))
        {
            magnitude += source.GetModifiedStats().Strength;
        }

        yield return ActivateEffect(targetActor, source.transform.position, magnitude);
    }

    private IEnumerator ActivateEffect(TileActor targetActor, Vector3? source, int actualMagnitude)
    {
        yield return RunAnimationEffects(targetActor, source);
        
        switch (EffectType)
        {
            case InstantEffectType.Heal:
                targetActor.DoHealing(actualMagnitude);
                break;
            case InstantEffectType.Damage:
                targetActor.DoDamage(actualMagnitude);
                break;
        }

        targetActor.AfterAppliedStatusEffect(this);
    }
}

public enum InstantEffectType
{
    Heal,
    Damage,
}

[Flags]
public enum ExtraEffectProperties
{
    None = 0,

    /// <summary>
    /// This ability gets a damage boost from the strength of the source.
    /// </summary>
    BoostedByStrength = 1,
}