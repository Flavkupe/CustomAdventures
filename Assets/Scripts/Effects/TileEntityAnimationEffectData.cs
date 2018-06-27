using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Tile Entity Effects", order = 2)]
public class TileEntityAnimationEffectData : AnimationEffectData
{
    public override Type AnimationEffectObjectType => typeof(TileEntityAnimationEffect);

    [Tooltip("Set to non-0 to apply color blink.")]
    public float BlinkColorSpeed = 0.0f;
    public Color BlinkColor;
}

