using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AnimationEffect : MonoBehaviourEx
{
    public abstract Routine Execute();
    public abstract Routine CreateRoutine();

    protected virtual void OnComplete()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Run after Data is set
    /// </summary>
    public virtual void InitEffect() {}

    public abstract void SetData(AnimationEffectData data);
}

public abstract class AnimationEffect<TDataType> : AnimationEffect where TDataType : AnimationEffectData
{
    public override Routine Execute()
    {
        var routine = this.CreateRoutine();
        StartCoroutine(routine);
        return routine;
    }

    public override Routine CreateRoutine()
    {
        var routine = Routine.Create(RunEffect);
        return routine;
    }

    public TDataType Data { get; set; }

    protected abstract IEnumerator RunEffectSequence();
    protected abstract IEnumerator RunEffectParallel();

    protected IEnumerator RunEffect()
    {
        switch (Data.SequenceType)
        {
            case AnimationEffectSequenceType.Sequence:
                yield return RunEffectSequence();
                break;
            case AnimationEffectSequenceType.Parallel:
                yield return RunEffectParallel();
                break;
        }
    }

    public override void SetData(AnimationEffectData data)
    {
        Debug.Assert(data is TDataType, "Data must be of type " + typeof(TDataType));        
        this.Data = data as TDataType;
    }
}

public abstract class AnimationEffectData : ScriptableObject
{
    public AnimationEffectData[] SubEffects;

    /// <summary>
    /// How long the effect will last for before disappearing. Not used if
    /// AllInnerEffects is selected for DurationType.
    /// </summary>
    public float Duration;

    public abstract Type AnimationEffectObjectType { get; }

    public AnimationEffectSequenceType SequenceType;

    public AnimationEffectDurationType DurationType;
}

public enum AnimationEffectSequenceType
{
    Sequence,
    Parallel
}

public enum AnimationEffectDurationType
{
    /// <summary>
    /// Wait until each inner animation is done, including particles etc
    /// </summary>
    AllInnerEffects,

    /// <summary>
    /// Wait until AnimationEffectData.Duration completes
    /// </summary>
    FixedDuration,
}