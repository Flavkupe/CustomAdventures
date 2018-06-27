using System.Collections;
using System.Linq;
using UnityEngine;

public class TileEntityAnimationEffect : AnimationEffect<TileEntityAnimationEffectData>
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
        Debug.Assert(TargetEntity != null, "Must call SetTargetEntity to use this type of animation effect!");

        OnBeforeExecute();

        if (TargetEntity != null && Data.BlinkColorSpeed > 0.0f)
        {
            TargetEntity.BlinkColor(Data.BlinkColor, Data.BlinkColorSpeed);
        }

        foreach (var effect in GetSubEffectAnimations())
        {
            emptyRoutineSet.AddRoutine(effect.CreateRoutine());
        }

        yield return emptyRoutineSet;

        OnComplete();
    }
}

