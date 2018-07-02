using System.Collections;
using UnityEngine;

/// <summary>
/// These are persistent effects which proc an effect when actions happen, such as poison or regen.
/// </summary>
[CreateAssetMenu(fileName = "Effect", menuName = "Create Status Effects/Proc Persistent Effect", order = 1)]
public class ProccingPersistentEffectData : PersistentEffectData
{
    [Tooltip("A Status effect that will happen after each proc")]
    public StatusEffectData StatusEffectData;

    [Tooltip("How many of the actions should be taken before the proc happens.")]
    public int ActionsToProc = 1;

    [Tooltip("Type of action that causes this effect apply")]
    public EffectActivatorType ProcType;

    protected override PersistentStatusEffect CreateEffect()
    {
        return new ProccingPersistentEffect(this);
    }

    public override IEnumerator ApplyEffectOn(TileActor targetActor, Vector3? source)
    {
        yield return RunAnimationEffects(targetActor, source);
        targetActor.AddStatusEffect(CreateEffect());
        targetActor.AfterAppliedStatusEffect(this);
    }
}

public class ProccingPersistentEffect : PersistentStatusEffect<ProccingPersistentEffectData>
{
    public ProccingPersistentEffect(ProccingPersistentEffectData data)
    {
        Data = data;
        _duration = data.Duration;
    }

    private int _duration;
    private int _actionsTaken = 0;

    public virtual void Proc(TileActor subject)
    {
        // TODO: do we want to block on this?
        subject.StartCoroutine(ProcEffectOn(subject));
    }

    public override void ActionTaken(TileActor subject, EffectActivatorType actionType)
    {
        if (Data.ProcType == EffectActivatorType.Any || actionType == Data.ProcType)
        {
            _actionsTaken++;
            if (_actionsTaken >= Data.ActionsToProc)
            {
                _actionsTaken = 0;
                Proc(subject);
            }
        }

        if (Data.ExpiryType == EffectActivatorType.Any || actionType == Data.ExpiryType)
        {
            _duration--;
            if (_duration <= 0)
            {
                Expire(subject);
            }
        }
    }

    public virtual IEnumerator ProcEffectOn(TileActor targetActor)
    {
        yield return Data.StatusEffectData.ApplyEffectOn(targetActor, targetActor);
    }
}
