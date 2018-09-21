using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallelRoutineSet : IRoutineSet
{
    private Func<IEnumerator, Coroutine> Runner => CoRoutineRunner.Instance.StartCoroutine;

    private readonly HashSet<Routine> _routines = new HashSet<Routine>();
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
        _routines.Add(routine);
    }

    public object Current => _func;

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
            routine.Finally(() => {
                _running--;
            });

            Runner(routine);
        }

        while (_running > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Factory for easily creating ParallelRoutineSet given a collection and a conversion
    /// from each item of that collection to a routine involving it.
    /// </summary>
    public static ParallelRoutineSet CreateSet<T>(IEnumerable<T> items, Func<T, Routine> func)
    {
        var set = new ParallelRoutineSet();
        foreach (var item in items)
        {
            set.AddRoutine(func(item));
        }

        return set;
    }
}

