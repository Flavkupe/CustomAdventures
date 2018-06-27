using System;
using System.Collections;
using System.Collections.Generic;
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
    protected Vector3? Target { get; set; }

    /// <summary>
    /// Optional source property, set for effects where applicable. This is the
    /// position where the effect is born from (player, amenity, enemy, etc). Some
    /// settings will always target source, or may fire a projectile from here.
    /// </summary>
    protected Vector3? Source { get; set; }

    /// <summary>
    /// Only needed if SetTargetEntity is not called
    /// </summary>
    public virtual void SetTargetPosition(Vector3? pos)
    {
        Target = pos;
    }

    /// <summary>
    /// Only needed if SetSourcePosition is not called
    /// </summary>
    public virtual void SetSourcePosition(Vector3? pos)
    {
        Source = pos;
    }
    protected TileEntity TargetEntity { get; private set; }
    protected TileEntity SourceEntity { get; private set; }

    public void SetTargetEntity(TileEntity entity)
    {
        if (entity != null)
        {
            Target = entity.transform.position;
            TargetEntity = entity;
        }
    }

    public void SetSourceEntity(TileEntity entity)
    {
        if (entity != null)
        {
            Source = entity.transform.position;
            SourceEntity = entity;
        }
    }
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

    protected List<AnimationEffect> GetSubEffectAnimations()
    {
        var effects = new List<AnimationEffect>();
        foreach (var data in Data.SubEffects)
        {
            var effect = GetEffectFromData(data);
            effects.Add(effect);
        }

        return effects;
    }

    protected AnimationEffect GetEffectFromData(AnimationEffectData data)
    {
        var effect = data.CreateEffect();
        effect.SetSourceEntity(SourceEntity);
        effect.SetTargetEntity(TargetEntity);
        effect.SetSourcePosition(Source);
        effect.SetTargetPosition(Target);
        return effect;
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
        effect.SetSourcePosition(source);
        effect.SetTargetPosition(target);
        return effect;
    }

    public static AnimationEffect GenerateTargetedAnimationEffect(AnimationEffectData data, TileEntity target, TileEntity source)
    {
        var effect = GenerateAnimationEffect(data);
        effect.SetSourceEntity(source);
        effect.SetTargetEntity(target);
        return effect;
    }
}

public static class AnimationEffectExtensionFunctions
{
    public static AnimationEffect CreateEffect(this AnimationEffectData data)
    {
        return AnimationEffectUtils.GenerateAnimationEffect(data);
    }

    public static AnimationEffect CreateTargetedEffect(this AnimationEffectData data, Vector3 target, Vector3 source)
    {
        return AnimationEffectUtils.GenerateTargetedAnimationEffect(data, target, source);
    }

    public static AnimationEffect CreateTargetedEffect(this AnimationEffectData data, TileEntity target, TileEntity source)
    {
        return AnimationEffectUtils.GenerateTargetedAnimationEffect(data, target, source);
    }
}