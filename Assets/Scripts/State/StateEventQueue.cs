using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EventQueueProcessor : MonoBehaviour
{
}

public class StateEventQueue
{
    private readonly Queue<Routine> _routineQueue = new Queue<Routine>();
    private bool _processingCoroutine;
    private EventQueueProcessor _processor;
    private readonly string _name;
    public StateEventQueue(string queueName)
    {
        _name = queueName;
    }

    public void Enqueue(Routine routine)
    {
        _routineQueue.Enqueue(routine);
    }

    public void Start()
    {
        // var obj = new GameObject($"{_name} Event Queue");
        
        _processor = Game.Player.gameObject.AddComponent<EventQueueProcessor>();
    }

    public void Update()
    {
        if (_processor == null)
        {
            Debug.LogError("Be sure to call Start on the StateController!");
            return;
        }

        if (!_processingCoroutine && _routineQueue.Count > 0)
        {
            _processor.StartCoroutine(InvokeNextQueuedCoroutine());
        }
    }

    private IEnumerator InvokeNextQueuedCoroutine()
    {
        // TODO: why is this not working??????
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
