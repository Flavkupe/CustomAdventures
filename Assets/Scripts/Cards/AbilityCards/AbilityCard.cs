
using System;
using UnityEngine;

public abstract class AbilityCard<T> : Card<T>, IAbilityCard where T : AbilityCardData
{
    public override CardType CardType { get { return CardType.Ability; } }
    public AbilityCardType AbilityCardType { get { return this.Data.AbilityCardType; } }
    public AbilityActivationType ActivationType { get { return this.Data.ActivationType; } }

    public Sprite AbilityIcon { get { return this.Data.AbilityIcon; } }

    // TODO: based on character class and card
    public bool ForgetOnUse { get { return true; } }

    public virtual void ActivateAbility()
    {
        // Must implement to use!
        throw new NotImplementedException();
    }

    protected void AfterCardUsed()
    {
        if (this.ForgetOnUse)
        {
            Game.Player.ForgetAbility(this);
        }
    }

    // Use this for initialization
    void Start ()
    {
	}

	// Update is called once per frame
	void Update ()
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