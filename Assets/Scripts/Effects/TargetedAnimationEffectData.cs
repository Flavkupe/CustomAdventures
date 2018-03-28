using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "CreateAction Animation Effects/Targeted Effect", order = 1)]
public class TargetedAnimationEffectData : AnimationEffectData
{
    public override Type AnimationEffectObjectType { get { return typeof(TargetedAnimationEffect); } }

    public AnimationEffectTargetType TargetType;
}

public enum AnimationEffectTargetType
{
    DefaultTarget,
    AlwaysTargetSource,
}

