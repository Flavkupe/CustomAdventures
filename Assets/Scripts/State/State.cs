using System;
using System.Collections;
using System.Collections.Generic;
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
