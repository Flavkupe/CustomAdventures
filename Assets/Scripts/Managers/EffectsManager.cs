using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EffectsManager : SingletonObject<EffectsManager>
{
    public TargetedAnimationEffect CreateTargetedAnimationEffect(TargetedAnimationEffectData data, Vector3 target)
    {
        var effect = GenerateAnimationEffect<TargetedAnimationEffect, TargetedAnimationEffectData>(data);
        effect.Target = target;
        return effect;
    }

    private void Awake()
    {
        Instance = this;
    }

    public AnimationEffect GenerateAnimationEffect(AnimationEffectData data)
    {
        var obj = new GameObject(data.name);
        var effect = obj.AddComponent(data.AnimationEffectObjectType) as AnimationEffect;
        effect.SetData(data);
        return effect;
    }

    public TEffectType GenerateAnimationEffect<TEffectType, TEffectDataType>(TEffectDataType data) where TEffectType : AnimationEffect<TEffectDataType> 
                                                                                                    where TEffectDataType : AnimationEffectData
    {
        var obj = new GameObject(data.name);
        var effect = obj.AddComponent<TEffectType>();
        effect.Data = data;
        return effect;
    }
}

