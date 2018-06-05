using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SoundGenerator))]
public abstract class AnimationEffect : MonoBehaviourEx
{
    public abstract Routine Execute();
    public abstract Routine CreateRoutine();

    protected virtual void OnComplete()
    {
        Destroy(gameObject);
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
        var routine = CreateRoutine();
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
        Data = data as TDataType;
    }

    protected virtual void OnBeforeExecute()
    {
        if (Data.InitSound != null)
        {
            GetComponent<SoundGenerator>().PlayClip(Data.InitSound);
        }
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

    public AudioClip InitSound;
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