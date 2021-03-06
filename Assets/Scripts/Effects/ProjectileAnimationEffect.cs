﻿using System.Collections;
using UnityEngine;

public class ProjectileAnimationEffect : AnimationEffect<ProjectileAnimationEffectData>
{
    private GameObject _projectile;

    public override void InitEffect()
    {
        base.InitEffect();

        if (Data.Projectile != null)
        {
            _projectile = Instantiate(Data.Projectile);
            _projectile.SetActive(true);
            _projectile.transform.parent = transform;
            _projectile.transform.localPosition = new Vector3();
        }
    }

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
        if (Target == null || Source == null)
        {
            Debug.Assert(Target != null, "No Target set!!");
            Debug.Assert(Target != null, "No Source set!!");
            OnComplete();
            yield break;
        }

        OnBeforeExecute();
        transform.position = Source.Value;
        IRoutineSet routines = GenerateRoutines(emptyRoutineSet);
        StartCoroutine(routines);
        yield return transform.MoveToSpotCoroutine(Target.Value, new MoveToSpotOptions
        {
            Speed = Data.ProjectileSpeed,
            AllowMouseSpeedup = true,
            RotationChange = Data.ProjectileRotation
        });

        if (Data.DestinationReachedEffect != null)
        {
            yield return OnReachedDestination();
        }

        OnComplete();
    }

    private IEnumerator OnReachedDestination()
    {
        if (Data.HideProjectileOnTargetReached && _projectile != null)
        {
            _projectile.SetActive(false);
        }

        if (Data.DestinationReachedEffect != null)
        {
            var effect = GetEffectFromData(Data.DestinationReachedEffect);
            effect.transform.parent = transform;
            effect.transform.localPosition = new Vector3();
            yield return effect.CreateRoutine();
        }
    }

    private IRoutineSet GenerateRoutines(IRoutineSet routines)
    {
        foreach (var effect in GetSubEffectAnimations())
        {
            effect.transform.SetParentAndPos(transform);
            routines.AddRoutine(effect.CreateRoutine());
        }

        return routines;
    }
}

