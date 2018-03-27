using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "CreateAction Animation Effects/Particle Effects", order = 2)]
public class ParticleAnimationEffectData : AnimationEffectData
{
    public ParticleSystem[] Particles;

    public override Type AnimationEffectObjectType { get { return typeof(ParticleAnimationEffect); } }
}