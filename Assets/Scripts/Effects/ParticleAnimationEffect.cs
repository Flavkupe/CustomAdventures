using System.Collections;
using System.Linq;
using UnityEngine;

public class ParticleAnimationEffect : AnimationEffect<ParticleAnimationEffectData>
{
    protected override IEnumerator RunEffectParallel()
    {
        OnBeforeExecute();
        float duration = Data.Duration;
        foreach (var particle in Data.Particles)
        {
            var obj = CreateParticle(particle);
            obj.Play();
        }

        if (Data.DurationType == AnimationEffectDurationType.Loop)
        {
            // If set to loop, this will never complete
            yield break;
        }

        if (Data.DurationType == AnimationEffectDurationType.AllInnerEffects)
        {
            duration = Data.Particles.Max(a => a.main.duration);
        }

        yield return Routine.WaitForSeconds(duration);

        OnComplete();
    }    

    protected override IEnumerator RunEffectSequence()
    {
        OnBeforeExecute();
        
        Debug.Assert(Data.DurationType != AnimationEffectDurationType.FixedDuration, "FixedDuration not supported here; use parallel");
        Debug.Assert(Data.DurationType != AnimationEffectDurationType.Loop, "Loop not supported here; use parallel");

        foreach (var particle in Data.Particles)
        {
            var obj = CreateParticle(particle);            
            obj.Play();
            yield return new WaitForSeconds(particle.main.duration);
        }

        OnComplete();
    }

    private ParticleSystem CreateParticle(ParticleSystem particle)
    {
        var obj = Instantiate(particle);
        obj.transform.parent = transform;
        obj.transform.position = transform.position;
        return obj;
    }
}

