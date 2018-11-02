using System;
using System.ComponentModel;

namespace Assets.Scripts.State
{
    /// <summary>
    /// Interface for objects that want to call the Updated function in
    /// response to some global event. NOTE: Do not do complicated actions
    /// in these, especially those involving game state! This is meant as an
    /// observer pattern for reactive components like the UI.
    /// </summary>
    public interface IUpdatesWhen
    {
        SimpleWorldEvent RequiredCondition { get; }

        void Updated();
    }
}
