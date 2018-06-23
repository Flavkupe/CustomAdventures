using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    // TODO: replace StatusEffect.cs with this stuff
    
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
}
