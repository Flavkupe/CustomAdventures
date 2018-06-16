using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Instant Effect", order = 1)]
public class InstantEffectData : StatusEffectData {

    [Tooltip("How strong the effect is, ie HP gained, damage taken per turn, etc")]
    public int Magnitude;

    public InstantEffectType EffectType;

    public override IEnumerator ApplyEffectOn(IDungeonActor actor)
    {
        if (AnimationEffect != null)
        {
            yield return AnimationEffect.CreateEffectRoutine();
        }

        switch (EffectType)
        {
            case InstantEffectType.Heal:
                actor.DoHealing(Magnitude);
                break;
            case InstantEffectType.Damage:
                actor.DoDamage(Magnitude);
                break;
        }

        actor.AfterAppliedStatusEffect(this);
        yield return null;
    }
}

public enum InstantEffectType
{
    Heal,
    Damage,
}