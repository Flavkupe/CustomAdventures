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
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Run after Data is set
    /// </summary>
    public virtual void InitEffect() {}

    public abstract void SetData(AnimationEffectData data);

    /// <summary>
    /// Optional target property; set for effects where applicable. This is
    /// a target position for effects like projectiles
    /// </summary>
    public Vector3? Target { get; set; }

    /// <summary>
    /// Optional source property, set for effects where applicable. This is the
    /// position where the effect is born from (player, amenity, enemy, etc). Some
    /// settings will always target source, or may fire a projectile from here.
    /// </summary>
    public Vector3? Source { get; set; }
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
    /// AllInnerEffects is selected for ActivatorType.
    /// </summary>
    public float Duration;

    public abstract Type AnimationEffectObjectType { get; }

    public AnimationEffectSequenceType SequenceType;

    public AudioClip InitSound;
}

public enum AnimationEffectSequenceType
{
    Sequence,
    Parallel
}

public static class AnimationEffectUtils
{
    public static Routine GenerateAnimationEffectRoutine(AnimationEffectData data)
    {
        var effect = GenerateAnimationEffect(data);
        return effect.CreateRoutine();
    }

    public static AnimationEffect GenerateAnimationEffect(AnimationEffectData data)
    {
        var obj = new GameObject(data.name);
        var effect = obj.AddComponent(data.AnimationEffectObjectType) as AnimationEffect;
        if (effect != null)
        {
            effect.SetData(data);
            effect.InitEffect();
        }

        return effect;
    }

    public static AnimationEffect GenerateTargetedAnimationEffect(AnimationEffectData data, Vector3 target, Vector3 source)
    {
        var effect = GenerateAnimationEffect(data);
        effect.Source = source;
        effect.Target = target;
        return effect;
    }
}

public static class AnimationEffectExtensionFunctions
{
    public static AnimationEffect CreateEffect(this AnimationEffectData data)
    {
        return AnimationEffectUtils.GenerateAnimationEffect(data);
    }

    public static Routine CreateEffectRoutine(this AnimationEffectData data)
    {
        return AnimationEffectUtils.GenerateAnimationEffectRoutine(data);
    }

    public static AnimationEffect CreateTargetedEffect(this AnimationEffectData data, Vector3 target, Vector3 source)
    {
        return AnimationEffectUtils.GenerateTargetedAnimationEffect(data, target, source);
    }
}