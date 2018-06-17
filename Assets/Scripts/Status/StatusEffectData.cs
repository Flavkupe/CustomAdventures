using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    // TODO: replace StatusEffect.cs with this stuff
    

    public AnimationEffectData AnimationEffect;

    public abstract IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source);
}
