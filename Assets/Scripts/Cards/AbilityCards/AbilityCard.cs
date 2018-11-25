using UnityEngine;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType => CardType.Ability;
    public AbilityCardType AbilityCardType => Data.AbilityCardType;
    public AbilityActivationType ActivationType => Data.ActivationType;

    public Sprite AbilityIcon => Data.AbilityIcon;

    // TODO: based on character class and card
    public bool ForgetOnUse => true;

    public AbilityUsageRequirementType UsageRequirement => Data.UsageRequirement;

    public abstract void ActivateAbility(GameContext context);

    protected void AfterCardUsed()
    {
        var player = Game.Player;
        player.AfterAbilityUsed(this);
        if (ForgetOnUse)
        {
            player.ForgetAbility(this);
        }
    }
}

public interface IAbilityCard : ICard
{
    AbilityCardType AbilityCardType { get; }
    AbilityActivationType ActivationType { get; }

    Sprite AbilityIcon { get; }

    AbilityUsageRequirementType UsageRequirement { get; }
    bool ForgetOnUse { get; }

    void ActivateAbility(GameContext context);
}

public enum AbilityUsageRequirementType
{
    FullAction,
    FullTurn,
    Free,
}

public enum AbilityActivationType
{
    Instant,
    TargetEntity,
}

public enum AbilityCardType
{
    Attack,
    SelfBuff,
}

public abstract class AbilityCardData : CardData
{
    public abstract AbilityCardType AbilityCardType { get; }
    public AbilityActivationType ActivationType;
    public Sprite AbilityIcon;
    public AbilityUsageRequirementType UsageRequirement;
}
