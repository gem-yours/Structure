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
    private List<SpellIcon> inactiveSpellIcons = new List<SpellIcon>();

    private int numberOfCandidates = 3;

    public Deck deck
    {
        set
        {
            value.onAdd = (Deck deck, Spell spell) =>
            {
                if (deck.isShuffling)
                {
                    return;
                }
                ShowSpell(spell);
            };
            value.onDraw = (SpellSlot slot, Spell spell) =>
            {
                StartCoroutine(OnDraw(value, spell, slot));
            };
            value.onProgress = (Spell spell, float progress, bool isActive) =>
            {
                var icon = inactiveSpellIcons.Find(x => x.spell == spell);
                if (icon == null) return;
                icon.fillAmount = 1 - progress;
                if (isActive)
                {
                    inactiveSpellIcons.Remove(icon);
                    icon.fillAmount = 0;
                }
            };
            value.onShuffle = (Deck deck) =>
            {
                var tmp = new List<SpellIcon>(spellIcons);
                foreach (SpellIcon spellIcon in tmp)
                {
                    HideSpell(spellIcon);
                }
                for (int i = 0; i < transform.childCount; i++)
                {
                    // たまにバグって何か残ることがある
                    // 対症療法だが、ずっと表示が壊れているよりは良いはず
                    Destroy(transform.GetChild(i).gameObject);
                }

                foreach (Spell? spell in deck.LatestCandidates(numberOfCandidates))
                {
                    ShowSpell(spell);
                }
            };

            value.onRemove = (SpellSlot slot) =>
            {
                UIManager.instance.UnsetSpell(slot);
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
        if (spellIcons.Count > numberOfCandidates - 1)
        {
            return;
        }
        var spellIcon = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        if (spellIcon == null)
        {
            return;
        }
        spellIcon.transform.localScale = Vector3.one;

        // 最後に追加したスペルが一番上に表示されるようにする
        for (var i = 0; i < spellIcons.Count; i++)
        {
            spellIcons[spellIcons.Count - 1 - i].transform.SetSiblingIndex(i + 1);
        }
        spellIcon.transform.SetSiblingIndex(0);

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

        yield return AnimationUtil.EaseInOut(
            0.25f,
            (float current) =>
            {
                fader.transform.localScale = new Vector3(1, current, 1);
            }
            );
        Destroy(fader.gameObject);
    }

    private IEnumerator OnDraw(Deck deck, Spell spell, SpellSlot slot)
    {
        var icon = spellIcons.Find(x => x.spell == spell);
        if (icon == null)
        {
            yield break;
        }
        icon.fillAmount = 1;
        inactiveSpellIcons.Add(icon);

        UIManager.instance.SetSpell(slot, icon); // TODO: UIManagerを直接呼び出すのはなんか汚い気がするので他の方法を検討する
        HideSpell(icon);

        yield return Fade();

        foreach (var candidate in deck.LatestCandidates(numberOfCandidates).Except(spellIcons.Select(x => x.spell)))
        {
            ShowSpell(candidate);
        }
        // 新たなスペルを表示する
        yield return null; // 最低1fは表示させないとサイズがおかしくなる
    }
}
