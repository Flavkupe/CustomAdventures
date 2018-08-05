using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StateEventQueue : MonoBehaviour
{
    private readonly Queue<Routine> _routineQueue = new Queue<Routine>();
    private bool _processingCoroutine;

    public void Enqueue(Routine routine)
    {
        _routineQueue.Enqueue(routine);
    }

    void Update()
    {
        if (!_processingCoroutine && _routineQueue.Count > 0)
        {
            StartCoroutine(InvokeNextQueuedCoroutine());
        }
    }

    private IEnumerator InvokeNextQueuedCoroutine()
    {
        if (_routineQueue.Count > 0)
        {
            Routine routine = _routineQueue.Dequeue();
            routine.Finally(() => _processingCoroutine = false);
            _processingCoroutine = true;
            yield return routine;
            _processingCoroutine = false;
        }
    }
}
