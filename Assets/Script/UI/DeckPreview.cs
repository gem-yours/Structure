using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class DeckPreview : MonoBehaviour
{
    private List<SpellIcon> spellIcons = new List<SpellIcon>();

    private int numberOfCandidates = 5;
    public Deck deck
    {
        set
        {
            value.onAdd = (Deck deck, Spell spell) =>
            {
                // 表示数が最大の場合は、デッキにカードが追加されても新たに表示する必要はない
                if (spellIcons.Count == numberOfCandidates)
                {
                    return;
                }
                ShowSpell(spell);

            };
            value.onDraw = (Deck deck, Spell spell) =>
            {
                // ドローされたカード=最新のカードを削除する
                var index = spellIcons.Select(x => x.spell).ToList().FindIndex(x => x == spell);
                if (index < 0)
                {
                    return;
                }

                UIManager.instance.SetSpell(spellIcons[index]);
                HideSpell(spellIcons[index]);

                // 新たなデッキトップのスペルを表示する
                var candidate = deck.LatestCandidates(numberOfCandidates).Last();
                ShowSpell(candidate);
            };

            value.onShuffle = (Deck deck) =>
            {
                var tmp = new List<SpellIcon>(spellIcons);
                foreach (SpellIcon spellIcon in tmp)
                {
                    HideSpell(spellIcon);
                }
                foreach (Spell? spell in deck.LatestCandidates(numberOfCandidates))
                {
                    ShowSpell(spell);
                }
            };

            foreach (Spell? spell in value.LatestCandidates(numberOfCandidates))
            {
                ShowSpell(spell);
            }
        }
    }


    private void ShowSpell(Spell? spell)
    {
        if (spell == null)
        {
            return;
        }
        var spellIcon = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        if (spellIcon == null)
        {
            return;
        }
        spellIcon.transform.localScale = Vector3.one;
        var si = spellIcon.GetComponent<SpellIcon>();
        si.spell = spell;
        spellIcons.Add(si);
    }

    private void HideSpell(SpellIcon spellIcon)
    {
        var index = spellIcons.FindIndex(x => x == spellIcon);
        if (index <= -1)
        {
            return;
        }
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
