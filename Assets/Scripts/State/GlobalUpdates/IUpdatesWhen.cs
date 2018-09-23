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

    public class GlobalObservable<T> : Observable<T> where T : IComparable
    {
        private readonly SimpleWorldEvent _eventCaused;

        public GlobalObservable(SimpleWorldEvent eventCaused)
        {
            _eventCaused = eventCaused;
            PropertyChanged += ObservableUpdater_PropertyChanged;
        }

        private void ObservableUpdater_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Game.World != null)
            {
                Game.World.SimpleEventHappened(_eventCaused);
            }
        }
    }
}
