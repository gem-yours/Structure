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

    private int numberOfCandidates = 2;
    public Deck deck
    {
        set
        {
            value.onAdd = (Deck deck, Spell spell) =>
            {
                // 表示数が最大の場合は、デッキにカードが追加されても新たに表示する必要はない
                if (spellIcons.Count >= numberOfCandidates)
                {
                    return;
                }
                ShowSpell(spell);

            };
            value.onDraw = (SpellSlot slot, Spell spell) =>
            {
                // ドローされたカード=最新のカードを削除する
                var icon = spellIcons.Last();
                UIManager.instance.SetSpell(slot, icon); // TODO: UIManagerを直接呼び出すのはなんか汚い気がするので他の方法を検討する
                HideSpell(icon);

                // 新たなデッキトップのスペルを表示する
                var candidate = value.LatestCandidates(numberOfCandidates).Last();
                ShowSpell(candidate);
            };

            value.onShuffle = (Deck deck) =>
            {
                var tmp = new List<SpellIcon>(spellIcons);
                foreach (SpellIcon spellIcon in tmp)
                {
                    HideSpell(spellIcon);
                }
                var reversed = deck.LatestCandidates(numberOfCandidates);
                reversed.Reverse(); // なんで反転がこんなに面倒くさいんですか？
                foreach (Spell? spell in reversed)
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

        var faderObj = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        if (faderObj == null) return;
        faderObj.transform.localScale = Vector3.one;

        var fader = faderObj.GetComponent<SpellIcon>();
        if (fader == null)
        {
            Destroy(faderObj);
            return;
        }
        StartCoroutine(Fade(fader));
    }

    // 1秒かけて小さくし、アニメーションを作る
    private IEnumerator Fade(SpellIcon icon)
    {
        var animationDuration = 0.25f;
        var easeInOut = AnimationCurve.EaseInOut(0, 1, animationDuration, 0);
        for (float current = 0; current <= animationDuration; current += Time.deltaTime)
        {
            icon.transform.localScale = new Vector3(1, easeInOut.Evaluate(current), 1);
            yield return null;
        }
        Destroy(icon.gameObject);
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
