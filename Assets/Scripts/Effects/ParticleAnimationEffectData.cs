using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Particle Effects", order = 2)]
public class ParticleAnimationEffectData : AnimationEffectData
{
    public ParticleSystem[] Particles;

    public override Type AnimationEffectObjectType { get { return typeof(ParticleAnimationEffect); } }
}