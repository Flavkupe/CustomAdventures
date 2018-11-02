using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[Serializable]
public class Observable<T> : INotifyPropertyChanged where T : IComparable
{
    public Observable()
    {
    }

    public Observable(T initialVal)
    {
        _value = initialVal;
    }

    public Observable(Observable<T> initial)
    {
        _value = initial.Value;
        PropertyChanged += initial.PropertyChanged;
    }

    /// <summary>
    /// Note: do not set this directly! This is meant only for the inspector. Use Value instead.
    /// </summary>
    public T _value;

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

    public virtual void Set(Observable<T> other, bool fireChangedEvent = false)
    {
        if (fireChangedEvent)
        {
            Value = other.Value;
        }
        else
        {
            _value = other.Value;
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

[Serializable]
public class IntObservable : Observable<int>
{
    public IntObservable()
    {
    }

    public IntObservable(int initialValue)
        : base (initialValue)
    {
    }

    public IntObservable(IntObservable initialValue)
        : base(initialValue)
    {
    }

    public static IntObservable operator ++(IntObservable left)
    {
        left.Value++;
        return left;
    }

    public static IntObservable operator --(IntObservable left)
    {
        left.Value--;
        return left;
    }

    public static IntObservable operator +(IntObservable left, IntObservable right)
    {
        left.Value += right.Value;
        return left;
    }

    public static IntObservable operator -(IntObservable left, IntObservable right)
    {
        left.Value -= right.Value;
        return left;
    }

    public static IntObservable operator +(IntObservable left, int right)
    {
        left.Value += right;
        return left;
    }

    public static int operator +(int left, IntObservable right)
    {
        return left + right.Value;
    }

    public static int operator -(int left, IntObservable right)
    {
        return left - right.Value;
    }


    public static IntObservable operator -(IntObservable left, int right)
    {
        left.Value -= right;
        return left;
    }


    public static bool operator >(IntObservable left, int right)
    {
        return left.Value > right;
    }

    public static bool operator <(IntObservable left, int right)
    {
        return left.Value < right;
    }

    public static bool operator <=(IntObservable left, int right)
    {
        return left.Value <= right;
    }

    public static bool operator >=(IntObservable left, int right)
    {
        return left.Value >= right;
    }

    public static bool operator ==(IntObservable left, int right)
    {
        return left.Value == right;
    }

    public static bool operator !=(IntObservable left, int right)
    {
        return left.Value != right;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static explicit operator int(IntObservable val)
    {
        return val.Value;
    }
}

