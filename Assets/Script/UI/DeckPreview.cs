using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckPreview : MonoBehaviour
{
    private List<Spell> _currentCandidates = new List<Spell> { null, null, null };

    public List<GameObject> candidates = new List<GameObject>();

    private int numberOfCandidates = 3;
    public Deck deck
    {
        set
        {
            if (value == null)
            {
                return;
            }
            value.onAdd = (Deck deck) =>
            {
                if (_currentCandidates.FindAll(x => x != null).Count >= 3)
                {
                    return;
                }
                var candidates = deck.LatestCandidates(numberOfCandidates);
                foreach (Spell spell in candidates)
                {
                    if (spell != null)
                    {
                        ShowSpell(spell);
                        _currentCandidates.Add(spell);
                    }
                }

            };
            // value.onPick = listener;
            // value.onShuffle = listener;
        }
    }


    private GameObject ShowSpell(Spell spell)
    {
        var spellIcon = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        spellIcon.transform.localScale = Vector3.one;
        spellIcon.GetComponent<SpellIcon>().spell = spell;
        return spellIcon;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
