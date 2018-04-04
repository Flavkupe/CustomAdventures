using System;
using UnityEngine;

/// <summary>
/// A timer that can be used to track cooldowns and 
/// durations of things.
/// </summary>
public class CooldownTimer
{
    /// <summary>
    /// The goal of the timer. Once this time is reached,
    /// the timer is "expired" (ie goes "ding!").
    /// </summary>
    private float _baseLine;

    private float _currentTime;

    public event EventHandler OnCooldownExpired;

    /// <summary>
    /// Ctor for creating an inactive timer. Set new baseline with
    /// SetBaseline method.
    /// </summary>
    public CooldownTimer()
    {
        _baseLine = 0.0f;
    }

    /// <summary>
    /// Ctor for creating an active timer with timeout
    /// occurring after time reaches 'baseLine'.
    /// </summary>
    /// <param name="baseLine">How long the timer must tick 
    /// before cooldown expires</param>
    public CooldownTimer(float baseLine)
    {
        _baseLine = baseLine;
    }

    /// <summary>
    /// Ticks the timer by some amount. Usually goes
    /// in the Update loop using Time.deltaTime, but
    /// delta can be anything. If delta is null, 
    /// Time.deltaTime is used. Returns a reference to
    /// this object, so you can do stuff like
    /// if (timer.Tick().IsExpired) and other such things.
    /// 
    /// Only ticks if the cooldown is active (not expired and
    /// non-zero baseLine). Immediately after expiry, the
    /// OnCooldownExpired event is fired.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public CooldownTimer Tick(float? delta = null)
    {
        if (IsActive)
        {
            _currentTime += delta ?? Time.deltaTime;
            if (IsExpired)
            {
                // On the exact tick after expiry, this event fires.
                if (OnCooldownExpired != null)
                {
                    OnCooldownExpired(this, new EventArgs());
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Whether or not the timer reached its goal
    /// </summary>
    public bool IsExpired { get { return _currentTime >= _baseLine; } }

    /// <summary>
    /// Whether or not this timer should tick.
    /// </summary>
    public bool IsActive { get { return _baseLine > 0.0f && !IsExpired; } }

    /// <summary>
    /// Resets the timer to be used again.
    /// </summary>
    public void Reset()
    {
        _currentTime = 0.0f;
    }

    /// <summary>
    /// Changes the target time for the timer.
    /// </summary>
    /// <param name="baseLine"></param>
    public void SetBaseline(float baseLine)
    {
        _baseLine = baseLine;
    }
}