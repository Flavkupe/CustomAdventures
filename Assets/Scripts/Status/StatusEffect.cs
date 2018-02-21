using System;

[Serializable]
public class StatusEffect
{
    public int Duration = 1;
    public EffectDurationType DurationType;
    public virtual void Apply(Player player)
    {
    }

    public virtual void Expire(Player player)
    {
    }
}

[Serializable]
public class SelfBuff : StatusEffect
{
    public int Strength = 0;
    public override void Apply(Player player)
    {
        player.Stats.BaseStrength += Strength;
    }

    public override void Expire(Player player)
    {
        player.Stats.BaseStrength -= Strength;
    }
}

/// <summary>
/// What causes the effect to go away
/// </summary>
public enum EffectDurationType
{
    Turns,
    Attacks,
    Steps,
}

/// <summary>
/// What procs persistent effects like poisons
/// </summary>
public enum EffectProcType
{
    Turns,
    Attacks,
    Steps,
}
