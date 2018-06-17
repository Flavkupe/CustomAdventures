using JetBrains.Annotations;
using UnityEngine;

public class EffectsManager : SingletonObject<EffectsManager>
{
    public AnimationEffect GenerateTargetedAnimationEffect(AnimationEffectData data, Vector3 target, Vector3 source)
    {
        var effect = GenerateAnimationEffect(data);
        effect.Source = source;
        effect.Target = target;
        return effect;
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }

    public Routine GenerateAnimationEffectRoutine(AnimationEffectData data)
    {
        var effect = GenerateAnimationEffect(data);
        return effect.CreateRoutine();
    }

    public AnimationEffect GenerateAnimationEffect(AnimationEffectData data)
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

    public TEffectType GenerateAnimationEffect<TEffectType, TEffectDataType>(TEffectDataType data) where TEffectType : AnimationEffect<TEffectDataType> 
                                                                                                    where TEffectDataType : AnimationEffectData
    {
        var obj = new GameObject(data.name);
        var effect = obj.AddComponent<TEffectType>();
        if (effect != null)
        {
            effect.SetData(data);
            effect.InitEffect();
        }

        return effect;
    }
}

