using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Targeted Effect", order = 1)]
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

