using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    [AssetIcon]
    [Tooltip("The icon that will appear for the status")]
    public Sprite StatusIcon;

    [Tooltip("Animation for the effect being applied")]
    public AnimationEffectData AnimationEffect;

    public abstract IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source);

    /// <summary>
    /// Use this override if the source is a TileActor. This may apply additional scaling to
    /// the effect, where appropriate.
    /// </summary>
    /// <param name="targetActor">Target of the effect.</param>
    /// <param name="source">Actor using this status effect.</param>
    public virtual IEnumerator ApplyEffectOn(TileActor targetActor, TileActor source)
    {
        yield return ApplyEffectOn(targetActor, source.transform.position);
    }

    protected virtual IEnumerator RunAnimationEffects(TileActor targetActor, Vector3? source = null)
    {
        if (AnimationEffect != null)
        {
            var effect = AnimationEffect.CreateEffect();
            effect.Source = source;
            effect.Target = targetActor.transform.position;
            yield return effect.CreateRoutine();
        }
    }
}
