using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonObject<GameManager>
{
    public bool IsPaused = false;

    public float MousedownSpeedMultiplier = 3.0f;

    public Dictionary<GameState, Queue<Action>> onStateChangeFromQueues = new Dictionary<GameState, Queue<Action>>();
    public Dictionary<GameState, Queue<Action>> onStateChangeToQueues = new Dictionary<GameState, Queue<Action>>();
    public Dictionary<GameState, Queue<Func<IEnumerator>>> onStateChangeFromCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();
    public Dictionary<GameState, Queue<Func<IEnumerator>>> onStateChangeToCoroutineQueues = new Dictionary<GameState, Queue<Func<IEnumerator>>>();

    public Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>> onTriggeredEventCoroutineQueues = new Dictionary<TriggeredEvent, Queue<Func<IEnumerator>>>();
    public Dictionary<TriggeredEvent, Queue<Action>> onTriggeredEventActionQueues = new Dictionary<TriggeredEvent, Queue<Action>>();

    private void EnqueueActionForQueue<R, T>(R state, T action, Dictionary<R, Queue<T>> dict)
    {        
        dict[state].Enqueue(action);
    }

    public void EnqueueAfterStateCoroutine(GameState afterStateChangedFrom, Func<IEnumerator> action)
    {        
        EnqueueActionForQueue(afterStateChangedFrom, action, onStateChangeFromCoroutineQueues);
    }

    public void EnqueueOnNewStateCoroutine(GameState afterStateChangedTo, Func<IEnumerator> action)
    {
        EnqueueActionForQueue(afterStateChangedTo, action, onStateChangeToCoroutineQueues);
    }

    public void EnqueueAfterStateAction(GameState afterStateChangedFrom, Action action)
    {
        if (afterStateChangedFrom == this.State)
        {
            EnqueueActionForQueue(afterStateChangedFrom, action, onStateChangeFromQueues);
        }
        else
        {
            action();
        }
    }

    public void EnqueueOnNewStateAction(GameState afterStateChangedTo, Action action)
    {
        // TODO: make this case conditional on params...?
        if (afterStateChangedTo != this.State)
        {
            EnqueueActionForQueue(afterStateChangedTo, action, onStateChangeToQueues);
        }
        else
        {
            action();
        }
    }

    public void EnqueueTriggeredEventAction(TriggeredEvent trigger, Action action)
    {
        EnqueueActionForQueue(trigger, action, onTriggeredEventActionQueues);
    }

    public void EnqueueTriggeredEventCoroutine(TriggeredEvent trigger, Func<IEnumerator> action)
    {
        EnqueueActionForQueue(trigger, action, onTriggeredEventCoroutineQueues);
    }

    private GameState _state;
    private GameState _lastState;

    public GameState State { get { return _state; } }

    public void SetState(GameState state)
    {
        if (state != _state)
        {            
            OnStateChanged(_state, state);
            _lastState = _state;
            _state = state;
            UIManager.Instance.UpdateUI();
        }    
    }

    public void RevertState()
    {
        this.SetState(_lastState);
    }

    public void TriggerEvent(TriggeredEvent triggeredEvent)
    {
        ProcessQueuedActions(onTriggeredEventActionQueues[triggeredEvent]);
        StartCoroutine(ProcessQueuedCoroutines(onTriggeredEventCoroutineQueues[triggeredEvent]));
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {        
        ProcessQueuedActions(onStateChangeToQueues[newState]);
        StartCoroutine(ProcessQueuedCoroutines(onStateChangeToCoroutineQueues[newState]));
        ProcessQueuedActions(onStateChangeFromQueues[oldState]);
        StartCoroutine(ProcessQueuedCoroutines(onStateChangeFromCoroutineQueues[oldState]));
    }

    private IEnumerator ProcessQueuedCoroutines(Queue<Func<IEnumerator>> queue)
    {
        while (queue.Count > 0)
        {
            Func<IEnumerator> action = queue.Dequeue();
            yield return StartCoroutine(action());
        }
    }

    private void ProcessQueuedActions(Queue<Action> queue)
    {
        while (queue.Count > 0)
        {
            Action action = queue.Dequeue();
            action();
        }
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;        
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            onStateChangeFromQueues[state] = new Queue<Action>();
            onStateChangeToQueues[state] = new Queue<Action>();            
            onStateChangeFromCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
            onStateChangeToCoroutineQueues[state] = new Queue<Func<IEnumerator>>();
        }

        foreach (TriggeredEvent trigger in Enum.GetValues(typeof(TriggeredEvent)))
        {
            onTriggeredEventCoroutineQueues[trigger] = new Queue<Func<IEnumerator>>();
            onTriggeredEventActionQueues[trigger] = new Queue<Action>();
        }
    }

    // Update is called once per frame
    void Update () {
	}

    public float GetMouseDownSpeedMultiplier()
    {
        return Input.GetMouseButton(0) ? this.MousedownSpeedMultiplier : 1.0f;
    }
}

public enum TriggeredEvent
{
    EntityTurnDone,
}

public enum GameState
{
    Neutral,
    DrawingCard,
    AwaitingCommand,
    CharacterMoving,
    EnemyTurn,
}