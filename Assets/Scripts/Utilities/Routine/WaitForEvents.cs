using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeAwaitEvent : CustomYieldInstruction
{
    private List<CustomYieldInstruction> _instructions = new List<CustomYieldInstruction>();    
    public CompositeAwaitEvent(params CustomYieldInstruction[] instructions)
    {
        _instructions.AddRange(instructions);
    }

    public override bool keepWaiting
    {
        get
        {
            return _instructions.Count >= 0 && _instructions.All(a => a.keepWaiting);
        }
    }
}

public class AwaitKeyPress : CustomYieldInstruction
{
    private HashSet<string> _validKeys = new HashSet<string>();

    public AwaitKeyPress()
    {
    }

    public AwaitKeyPress(params string[] validKeys)
    {
        validKeys.ToList().ForEach(a => _validKeys.Add(a));
    }

    public string KeyPressed { get; set; }
    public bool Activated { get; set; }
    public override bool keepWaiting
    {
        get
        {
            if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString) && 
                (_validKeys.Count == 0 || _validKeys.Contains(Input.inputString)))
            {
                KeyPressed = Input.inputString;
                Activated = true;
                return false;
            }

            return true;
        }
    }
}

public class AwaitTriggerEvent<T> : CustomYieldInstruction where T : struct
{
    public T EventValue { get; set; }
    public bool Activated { get; set; }
    private Func<T?> _eventChecker = null;
    private HashSet<T> _validTriggers = new HashSet<T>();

    public AwaitTriggerEvent(Func<T?> eventChecker) {
        _eventChecker = eventChecker;
    }

    public AwaitTriggerEvent(Func<T?> eventChecker, params T[] validTriggers)
    {
        _eventChecker = eventChecker;
        validTriggers.ToList().ForEach(a => _validTriggers.Add(a));
    }

    public override bool keepWaiting
    {
        get
        {
            T? current = _eventChecker();
            if (current != null && (_validTriggers.Count == 0 || _validTriggers.Contains(current.Value)))
            {
                EventValue = current.Value;
                Activated = true;
                return false;
            }

            return true;
        }
    }
}
