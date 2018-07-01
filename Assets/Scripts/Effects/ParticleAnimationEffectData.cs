using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Create Animation Effects/Particle Effects", order = 2)]
public class ParticleAnimationEffectData : AnimationEffectData
{
    public ParticleSystem[] Particles;

    [Tooltip("Parents the particles to apropriate target.")]
    public ParticleEffectTargeting Targeting;

    public override Type AnimationEffectObjectType => typeof(ParticleAnimationEffect);

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

public enum ParticleEffectTargeting
{
    /// <summary>
    /// Don't parent to anything; let parent effects decide (such as for projectiles)
    /// </summary>
    Default,

    /// <summary>
    /// Always put position on Target
    /// </summary>
    ParentParticlesToTarget,

    /// <summary>
    /// Always put position on Source
    /// </summary>
    ParentParticlesToSource,
}

