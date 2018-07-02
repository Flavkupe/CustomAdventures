using System;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public Transition[] Transitions;
}

[Serializable]
public class Transition
{
    public Decision Decision;
    public State Next;
}
