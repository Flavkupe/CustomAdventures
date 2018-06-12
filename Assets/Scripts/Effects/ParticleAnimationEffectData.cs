﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Particle Effects", order = 2)]
public class ParticleAnimationEffectData : AnimationEffectData
{
    public ParticleSystem[] Particles;

    public override Type AnimationEffectObjectType { get { return typeof(ParticleAnimationEffect); } }

    public AnimationDurationType DurationType;

    public enum AnimationDurationType
    {
        /// <summary>
        /// Wait until each inner animation is done, including particles etc
        /// </summary>
        AllInnerEffects,

        /// <summary>
        /// Wait until AnimationEffectData.Duration completes
        /// </summary>
        FixedDuration,

        /// <summary>
        /// Where applicable, this effect has no duration, looping until destroyed from elsewhere.
        /// </summary>
        Loop,
    }
}

