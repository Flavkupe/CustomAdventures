using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{
    public class Observable<T> : INotifyPropertyChanged where T : IComparable
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                if (value.Equals(_value)) return;
                _value = value;
                OnPropertyChanged(nameof(_value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
