using System.Collections;
using System.Linq;
using UnityEngine;

public class ParticleAnimationEffect : AnimationEffect<ParticleAnimationEffectData>
{
    protected override void OnBeforeExecute()
    {
        base.OnBeforeExecute();

        // Set the position to either the Target or Source if either are used
        if (Target != null)
        {
            transform.position = Target.Value;
        }
        else if (Source != null)
        {
            transform.position = Source.Value;
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
            effect.transform.parent = transform;
            effect.transform.position = transform.position;
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
            effect.transform.parent = transform;
            effect.transform.position = transform.position;
            yield return effect.CreateRoutine();
        }

        OnComplete();
    }

    private ParticleSystem CreateParticle(ParticleSystem particle)
    {
        var obj = Instantiate(particle);
        var parent = Data.ParentParticlesToTarget && TargetEntity != null ? TargetEntity.transform : transform;
        obj.transform.parent = parent;
        obj.transform.position = parent.position;
        return obj;
    }
}

