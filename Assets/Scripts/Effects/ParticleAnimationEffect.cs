using System.Collections;
using System.Linq;
using UnityEngine;

public class ParticleAnimationEffect : AnimationEffect<ParticleAnimationEffectData>
{
    protected override IEnumerator RunEffectParallel()
    {       
        float duration = Data.Duration;
        foreach (var particle in Data.Particles)
        {
            var obj = CreateParticle(particle);
            obj.Play();
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
        // TODO: duration-based
        Debug.Assert(Data.DurationType != AnimationEffectDurationType.FixedDuration, "FixedDuration not supported here");        

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

