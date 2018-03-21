using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallelRoutineSet : IEnumerator, IRoutineSet
{
    private HashSet<Routine> _routines = new HashSet<Routine>();
    private IEnumerator _func = null;
    private int _running = 0;

    public ParallelRoutineSet()
    {
    }

    public ParallelRoutineSet(params Routine[] routines)
    {        
        _routines.UnionWith(routines);
    }

    public ParallelRoutineSet(params Func<IEnumerator>[] routines)
    {     
        _routines.UnionWith(routines.Select(a => Routine.Create(a)));
    }

    public Routine AsRoutine()
    {
        return Routine.Create(() => { return this; });
    }

    public void AddRoutine(Routine routine)
    {
        this._routines.Add(routine);
    }

    public object Current
    {
        get { return _func; }
    }

    public bool MoveNext()
    {
        if (_func == null)
        {
            _func = Execute();
            return true;
        }

        return false;
    }

    public void Reset()
    {
    }

    private IEnumerator Execute()
    {
        _running = _routines.Count;
        foreach (var routine in _routines)
        {
            // TODO: this replaces the current finally!!
            routine.Finally(() => {
                _running--;
            });

            Game.States.StartCoroutine(routine);
        }

        while (_running > 0)
        {
            yield return null;
        }
    }
}

