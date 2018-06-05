﻿using System.Collections;
using UnityEngine;

public class ProjectileAnimationEffect : AnimationEffect<ProjectileAnimationEffectData>
{
    public Vector3 Target { get; set; }

    public Vector3 Source { get; set; }

    private GameObject _projectile;

    public override void InitEffect()
    {
        base.InitEffect();

        if (Data.Projectile != null)
        {
            _projectile = Instantiate(Data.Projectile);
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
        OnBeforeExecute();
        transform.position = Source;
        IRoutineSet routines = GenerateRoutines(emptyRoutineSet);
        StartCoroutine(routines);
        yield return transform.MoveToSpotCoroutine(Target, Data.ProjectileSpeed);

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
            var effect = Game.Effects.GenerateAnimationEffect(Data.DestinationReachedEffect);
            effect.transform.parent = transform;
            effect.transform.localPosition = new Vector3();
            yield return effect.CreateRoutine();
        }
    }

    private IRoutineSet GenerateRoutines(IRoutineSet routines)
    {
        foreach (var data in Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.parent = transform;
            effect.transform.position = transform.position;
            routines.AddRoutine(effect.CreateRoutine());
        }

        return routines;
    }
}

