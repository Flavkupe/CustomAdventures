using System.Collections;
using UnityEngine;

public class TargetedAnimationEffect : AnimationEffect<TargetedAnimationEffectData>
{
    public Vector3 Target { get; set; }

    public Vector3 Source { get; set; }


    protected override IEnumerator RunEffectParallel()
    {
        yield return Execute(new ParallelRoutineSet());
    }

    protected override IEnumerator RunEffectSequence()
    {
        yield return Execute(new RoutineChain());
    }

    private IEnumerator Execute(IRoutineSet emptyRoutineSet)
    {
        OnBeforeExecute();
        foreach (var data in Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.position = GetTarget();
            emptyRoutineSet.AddRoutine(effect.CreateRoutine());
        }

        yield return emptyRoutineSet;

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

