using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DeckPreview : MonoBehaviour
{
    private List<SpellIcon> spellIcons = new List<SpellIcon>();

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
                if (spellIcons.Select(x => x.spell).ToList().FindAll(x => x != null).Count >= 3)
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
                    }
                }

            };
            value.onDraw = (Deck deck, Spell spell) =>
            {
                // ドローされたカード=最新のカードを削除する
                var index = spellIcons.Select(x => x.spell).ToList().FindIndex(x => x == spell);
                if (index > -1)
                {
                    HideSpell(spellIcons[index]);
                }

                // 新たなデッキトップのスペルを表示する
                var candidates = deck.LatestCandidates(numberOfCandidates);
                foreach (Spell newSpell in candidates.Except(spellIcons.Select(x => x.spell).ToList()))
                {
                    ShowSpell(newSpell);
                }
            };
            value.onShuffle = (Deck deck) =>
                {
                    foreach (SpellIcon spellIcon in spellIcons)
                    {
                        HideSpell(spellIcon);
                    }
                    foreach (Spell spell in deck.LatestCandidates(numberOfCandidates))
                    {
                        ShowSpell(spell);
                    }
                };
        }
    }


    private void ShowSpell(Spell spell)
    {
        var spellIcon = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
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
            Debug.Log("not found");
            return;
        }
        Destroy(spellIcons[index].gameObject);
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
