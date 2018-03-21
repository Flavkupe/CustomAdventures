using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ProjectileAnimationEffect : AnimationEffect<ProjectileAnimationEffectData>
{
    public Vector3 Target { get; set; }

    public Vector3 Source { get; set; }

    private GameObject _projectile = null;

    public override void InitEffect()
    {
        base.InitEffect();

        if (this.Data.Projectile != null)
        {
            _projectile = Instantiate(this.Data.Projectile);
            _projectile.transform.parent = this.transform;
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
        this.transform.position = this.Source;
        IRoutineSet routines = GenerateRoutines(emptyRoutineSet);
        StartCoroutine(routines);
        yield return this.transform.MoveToSpotCoroutine(this.Target, this.Data.ProjectileSpeed);

        if (this.Data.DestinationReachedEffect != null)
        {
            yield return OnReachedDestination();
        }

        this.OnComplete();
    }

    private IEnumerator OnReachedDestination()
    {
        if (this.Data.HideProjectileOnTargetReached && this._projectile != null)
        {
            this._projectile.SetActive(false);
        }

        if (this.Data.DestinationReachedEffect != null)
        {
            var effect = Game.Effects.GenerateAnimationEffect(this.Data.DestinationReachedEffect);
            effect.transform.parent = this.transform;
            effect.transform.localPosition = new Vector3();
            yield return effect.CreateRoutine();
        }
    }

    private IRoutineSet GenerateRoutines(IRoutineSet routines)
    {
        foreach (var data in this.Data.SubEffects)
        {
            var effect = Game.Effects.GenerateAnimationEffect(data);
            effect.transform.parent = this.transform;
            effect.transform.position = this.transform.position;
            routines.AddRoutine(effect.CreateRoutine());
        }

        return routines;
    }
}

