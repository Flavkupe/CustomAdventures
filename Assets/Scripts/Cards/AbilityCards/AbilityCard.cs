using UnityEngine;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType => CardType.Ability;
    public AbilityCardType AbilityCardType => Data.AbilityCardType;
    public AbilityActivationType ActivationType => Data.ActivationType;

    public Sprite AbilityIcon => Data.AbilityIcon;

    // TODO: based on character class and card
    public bool ForgetOnUse => true;

    public abstract void ActivateAbility(GameContext context);

    protected void AfterCardUsed()
    {
        Game.Player.AfterAbilityUsed(this);
        if (ForgetOnUse)
        {
            Game.Player.ForgetAbility(this);
        }
    }
}

public interface IAbilityCard : ICard
{
    AbilityCardType AbilityCardType { get; }
    AbilityActivationType ActivationType { get; }

    Sprite AbilityIcon { get; }

    bool ForgetOnUse { get; }

    void ActivateAbility(GameContext context);
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
}
