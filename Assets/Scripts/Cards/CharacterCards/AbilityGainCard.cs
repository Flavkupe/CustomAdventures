public class AbilityGainCard : CharacterCard<AbilityGainCardData>
{
    public override void ApplyEffect()
    {
        // TODO: Animation to shuffle cards into the deck
        for (int i = 0; i < Data.NumberGained; i++)
        {
            IAbilityCard card = Game.Decks.CreateCardFromData<IAbilityCard, AbilityCardData>(Data.AbilityGained);
            Game.Decks.AbilityDeck.PushCard(card);
        }
    }
}

