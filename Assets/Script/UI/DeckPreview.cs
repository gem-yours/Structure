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
                StartCoroutine(OnDraw(value, slot));
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
        spellIcon.transform.SetSiblingIndex(spellIcons.Count - 1);
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

    // 1秒かけて小さくし、アニメーションを作る
    private IEnumerator Fade()
    {
        var faderObj = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        if (faderObj == null) yield break;
        faderObj.transform.localScale = Vector3.one;
        faderObj.transform.SetSiblingIndex(transform.childCount);

        var fader = faderObj.GetComponent<SpellIcon>();
        if (fader == null)
        {
            Destroy(faderObj);
            yield break;
        }
        var animationDuration = 0.25f;
        var easeInOut = AnimationCurve.EaseInOut(0, 1, animationDuration, 0);
        for (float current = 0; current <= animationDuration; current += Time.deltaTime)
        {
            fader.transform.localScale = new Vector3(1, easeInOut.Evaluate(current), 1);
            yield return null;
        }
        Destroy(fader.gameObject);
    }

    private IEnumerator OnDraw(Deck deck, SpellSlot slot)
    {
        // ドローされたカードを候補から削除する
        var icon = spellIcons.Last();
        UIManager.instance.SetSpell(slot, icon); // TODO: UIManagerを直接呼び出すのはなんか汚い気がするので他の方法を検討する
        HideSpell(icon);

        yield return Fade();

        // 新たなデッキトップのスペルを表示する
        var candidate = deck.LatestCandidates(numberOfCandidates).Last();
        ShowSpell(candidate);
    }
}
