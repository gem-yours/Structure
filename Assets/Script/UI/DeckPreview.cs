using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DeckPreview : MonoBehaviour
{
    private List<Spell> _currentCandidates = new List<Spell> { null, null, null };

    public List<GameObject> spellIcons = new List<GameObject>();

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
                // デッキの枚数が3枚以上ある場合は、デッキにカードが追加されても新たに表示する必要はない
                if (_currentCandidates.FindAll(x => x != null).Count >= 3)
                {
                    return;
                }
                var candidates = deck.LatestCandidates(numberOfCandidates);
                // デッキトップが下に表示されるようにする
                candidates.Reverse();
                foreach (Spell spell in candidates)
                {
                    if (spell != null)
                    {
                        ShowSpell(spell);
                        _currentCandidates.Add(spell);
                    }
                }

            };
            value.onPick = (Deck deck) =>
            {
                HideSpell(_currentCandidates[0]);

                var spell = deck.LatestCandidates(numberOfCandidates).Last();
                if (spell != null)
                {
                    _currentCandidates.Add(spell);
                    ShowSpell(spell);
                }
            };
            value.onShuffle = (Deck deck) =>
            {
                _currentCandidates.RemoveAll(x => true);
                foreach (Spell spell in _currentCandidates)
                {
                    HideSpell(spell);
                }
            };
        }
    }


    private void ShowSpell(Spell spell)
    {
        var spellIcon = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        spellIcon.transform.localScale = Vector3.one;
        spellIcon.GetComponent<SpellIcon>().spell = spell;
        spellIcons.Add(spellIcon);
    }

    private void HideSpell(Spell spell)
    {
        var index = _currentCandidates.FindIndex(x => x == spell);
        if (index == -1)
        {
            return;
        }
        _currentCandidates.RemoveAt(index);
        Destroy(spellIcons[index]);
        spellIcons.RemoveAt(index);
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
