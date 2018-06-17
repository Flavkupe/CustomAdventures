using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Instant Effect", order = 1)]
public class InstantEffectData : StatusEffectData {

    [Tooltip("How strong the effect is, ie HP gained, damage taken per turn, etc")]
    public int Magnitude;

    public InstantEffectType EffectType;

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        if (AnimationEffect != null)
        {
            var effect = AnimationEffect.CreateEffect();
            effect.Source = source;
            effect.Target = targetActor.transform.position;
            yield return effect.CreateRoutine();
        }

        switch (EffectType)
        {
            case InstantEffectType.Heal:
                targetActor.DoHealing(Magnitude);
                break;
            case InstantEffectType.Damage:
                targetActor.DoDamage(Magnitude);
                break;
        }

        targetActor.AfterAppliedStatusEffect(this);
        yield return null;
    }
}

public enum InstantEffectType
{
    Heal,
    Damage,
}