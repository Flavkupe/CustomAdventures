
using System;
using JetBrains.Annotations;
using UnityEngine;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType { get { return CardType.Ability; } }
    public AbilityCardType AbilityCardType { get { return Data.AbilityCardType; } }
    public AbilityActivationType ActivationType { get { return Data.ActivationType; } }

    public Sprite AbilityIcon { get { return Data.AbilityIcon; } }

    // TODO: based on character class and card
    public bool ForgetOnUse { get { return true; } }

    public virtual void ActivateAbility()
    {
        // Must implement to use!
        throw new NotImplementedException();
    }

    protected void AfterCardUsed()
    {
        if (ForgetOnUse)
        {
            Game.Player.ForgetAbility(this);
        }
    }

    [UsedImplicitly]
    private void Start ()
    {
	}

    [UsedImplicitly]
    private void Update ()
    {
	}
}

public interface IAbilityCard : ICard
{
    AbilityCardType AbilityCardType { get; }
    AbilityActivationType ActivationType { get; }

    Sprite AbilityIcon { get; }

    bool ForgetOnUse { get; }

    void ActivateAbility();
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