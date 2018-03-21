using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TargetedAnimationEffect : AnimationEffect<TargetedAnimationEffectData>
{
    public Vector3 Target { get; set; }

    public Vector3 Source { get; set; }

    protected override IEnumerator RunEffectParallel()
    {
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var data in this.Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.position = GetTarget();
            routines.AddRoutine(effect.CreateRoutine());
        }

        yield return routines;

        this.OnComplete();
    }

    protected override IEnumerator RunEffectSequence()
    {
        foreach (var data in this.Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.position = GetTarget();
            yield return effect.CreateRoutine();
        }
                
        this.OnComplete();
    }

    private Vector3 GetTarget()
    {
        switch (this.Data.TargetType)
        {
            case AnimationEffectTargetType.AlwaysTargetSource:
                return Game.Player.transform.position;
            case AnimationEffectTargetType.DefaultTarget:
            default:
                return this.Target;
        }
    }

}

