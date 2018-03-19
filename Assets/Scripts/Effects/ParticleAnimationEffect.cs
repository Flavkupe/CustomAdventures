using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleAnimationEffect : AnimationEffect<ParticleAnimationEffectData>
{
    protected override IEnumerator RunEffectParallel()
    {       
        float duration = this.Data.Duration;
        foreach (var particle in this.Data.Particles)
        {
            var obj = CreateParticle(particle);
            obj.Play();
        }

        if (this.Data.DurationType == AnimationEffectDurationType.AllInnerEffects)
        {
            duration = this.Data.Particles.Max(a => a.main.duration);
        }

        yield return Routine.WaitForSeconds(duration);

        this.OnComplete();
    }    

    protected override IEnumerator RunEffectSequence()
    {
        // TODO: duration-based
        Debug.Assert(this.Data.DurationType != AnimationEffectDurationType.FixedDuration, "FixedDuration not supported here");        

        foreach (var particle in this.Data.Particles)
        {
            var obj = CreateParticle(particle);            
            obj.Play();
            yield return new WaitForSeconds(particle.main.duration);
        }

        this.OnComplete();
    }

    private ParticleSystem CreateParticle(ParticleSystem particle)
    {
        var obj = Instantiate(particle);
        obj.transform.parent = this.transform;
        obj.transform.position = this.transform.position;
        return obj;
    }
}

