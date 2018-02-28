public class AbilityGainCard : CharacterCard<AbilityGainCardData>
{
    public override void ApplyEffect()
    {
        // TODO: Animation to shuffle cards into the deck
        for (int i = 0; i < this.Data.NumberGained; i++)
        {
            IAbilityCard card = Game.Decks.CreateCardFromData<IAbilityCard, AbilityCardData>(this.Data.AbilityGained);
            Game.Decks.AbilityDeck.PushCard(card);
        }
    }
}

