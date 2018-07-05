using System.Collections;

public class AbilityGainCard : CharacterCard<AbilityGainCardData>
{
    public override IEnumerator ApplyEffect(CharacterCardExecutionContext context)
    {
        // TODO: Animation to shuffle cards into the deck
        for (int i = 0; i < Data.NumberGained; i++)
        {
            IAbilityCard card = context.Decks.CreateCardFromData<IAbilityCard, AbilityCardData>(Data.AbilityGained);
            context.Decks.AbilityDeck.PushCard(card);
        }

        yield return null;
    }
}

