using System.Collections;
using UnityEngine;

public class TargetedAnimationEffect : AnimationEffect<TargetedAnimationEffectData>
{
    public Vector3 Target { get; set; }

    public Vector3 Source { get; set; }

    protected override IEnumerator RunEffectParallel()
    {
        OnBeforeExecute();
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var data in Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.position = GetTarget();
            routines.AddRoutine(effect.CreateRoutine());
        }

        yield return routines;

        OnComplete();
    }

    protected override IEnumerator RunEffectSequence()
    {
        OnBeforeExecute();
        foreach (var data in Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.position = GetTarget();
            yield return effect.CreateRoutine();
        }

        OnComplete();
    }

    private Vector3 GetTarget()
    {
        switch (Data.TargetType)
        {
            case AnimationEffectTargetType.AlwaysTargetSource:
                return Game.Player.transform.position;
            case AnimationEffectTargetType.DefaultTarget:
            default:
                return Target;
        }
    }

}

