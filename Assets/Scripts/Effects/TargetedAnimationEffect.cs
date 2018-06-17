using System.Collections;
using UnityEngine;

public class TargetedAnimationEffect : AnimationEffect<TargetedAnimationEffectData>
{
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
        Debug.Assert(Target != null && Source != null, "No Source or Target set!!");
        switch (Data.TargetType)
        {
            case AnimationEffectTargetType.AlwaysTargetSource:
                return Source ?? Game.Player.transform.position;
            case AnimationEffectTargetType.DefaultTarget:
            default:
                return Target ?? Source ?? this.transform.position;
        }
    }

}

