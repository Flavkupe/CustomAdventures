using System.Collections;
using System.Linq;
using UnityEngine;

public class ParticleAnimationEffect : AnimationEffect<ParticleAnimationEffectData>
{
    private ParticleSystem _particles;

    protected override void OnBeforeExecute()
    {
        base.OnBeforeExecute();

        switch (Data.Targeting)
        {
            case ParticleEffectTargeting.ParentParticlesToTarget:
                if (TargetEntity != null)
                {
                    transform.SetParentAndPos(TargetEntity.transform);
                }
                else if (Target != null)
                {
                    transform.position = Target.Value;
                }

                break;
            case ParticleEffectTargeting.ParentParticlesToSource:
                if (SourceEntity != null)
                {
                    transform.SetParentAndPos(SourceEntity.transform);
                }
                else if (Source != null)
                {
                    transform.position = Source.Value;
                }

                break;
        }
    }

    protected override IEnumerator RunEffectParallel()
    {
        OnBeforeExecute();
        float duration = Data.Duration;
        foreach (var particle in Data.Particles)
        {
            var obj = CreateParticle(particle);
            obj.Play();
        }

        if (Data.DurationType == ParticleAnimationEffectData.AnimationDurationType.Loop)
        {
            // If set to loop, this will never complete
            yield break;
        }

        if (Data.DurationType == ParticleAnimationEffectData.AnimationDurationType.AllInnerEffects)
        {
            duration = Data.Particles.Max(a => a.main.duration);
        }

        foreach (var effect in GetSubEffectAnimations())
        {
            // Run effects in parallel
            effect.transform.SetParentAndPos(transform);
            StartCoroutine(effect.CreateRoutine());
        }

        yield return Routine.WaitForSeconds(duration);

        OnComplete();
    }

    protected override IEnumerator RunEffectSequence()
    {
        OnBeforeExecute();
        
        Debug.Assert(Data.DurationType != ParticleAnimationEffectData.AnimationDurationType.FixedDuration, "FixedDuration not supported here; use parallel");
        Debug.Assert(Data.DurationType != ParticleAnimationEffectData.AnimationDurationType.Loop, "Loop not supported here; use parallel");

        foreach (var particle in Data.Particles)
        {
            var obj = CreateParticle(particle);            
            obj.Play();
            yield return new WaitForSeconds(particle.main.duration);
        }

        foreach (var effect in GetSubEffectAnimations())
        {
            // Run sub-effects in sequence
            effect.transform.SetParentAndPos(transform);
            yield return effect.CreateRoutine();
        }

        OnComplete();
    }

    protected override void OnComplete()
    {
        base.OnComplete();
    }

    private ParticleSystem CreateParticle(ParticleSystem particle)
    {
        _particles = Instantiate(particle);
        _particles.transform.SetParentAndPos(transform);
        return _particles;
    }
}

