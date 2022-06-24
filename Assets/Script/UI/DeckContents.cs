using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class DeckContents : MonoBehaviour
{
#pragma warning disable CS8618
    public GridLayoutGroup gridLayoutGroup;
#pragma warning restore CS8618

    private Deck? _deck = null;
    public Deck? deck
    {
        get
        {
            return _deck;
        }
        set
        {
            _deck = value;
            ShowDeck(value);
        }
    }

    private List<SpellCard> spellCards = new List<SpellCard>();

    private void ShowDeck(Deck? deck)
    {
        if (deck is null) return;
        foreach (var index in Enumerable.Range(0, Mathf.Max(deck.spells.Count, spellCards.Count)))
        {
            var spell = (index < deck.spells.Count) ? deck.spells[index] : null;

            SpellCard? spellCard = null;
            if (index < spellCards.Count)
            {
                spellCard = spellCards[index];
            }
            else
            {
                spellCard = (Instantiate(Resources.Load("SpellIcon/SpellCard"), gridLayoutGroup.transform) as GameObject)?.GetComponent<SpellCard>();
                if (spellCard is not null) spellCards.Add(spellCard);
            }
            if (spellCard == null) return;
            spellCard.spell = spell;
        }
    }
}
