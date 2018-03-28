using UnityEngine;

public class WaitForSecondsSpeedable : CustomYieldInstruction
{
    private float _seconds = 0.0f;
    public WaitForSecondsSpeedable(float seconds)
    {
        _seconds = seconds;
    }

    public override bool keepWaiting
    {
        get
        {
            _seconds -= Time.deltaTime * Game.States.GetMouseDownSpeedMultiplier();
            return _seconds > 0.0f;
        }
    }
}

