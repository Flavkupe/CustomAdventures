using System;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Projectile Effect", order = 1)]
public class ProjectileAnimationEffectData : TargetedAnimationEffectData
{
    [Description("Effect that happens when projectile reaches destination")]
    public AnimationEffectData DestinationReachedEffect;

    public override Type AnimationEffectObjectType { get { return typeof(ProjectileAnimationEffect); } }

    public bool HideProjectileOnTargetReached = true;

    public GameObject Projectile;

    public float ProjectileSpeed = 10.0f;
}
