using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

#nullable enable
public class SpellCard : MonoBehaviour
{
    private Spell? _spell = null;
    public Spell? spell
    {
        set
        {
            _spell = value;

            if (value is not null)
            {
                spellImage.sprite = value.image;
                spellDescription.text = value.description;
                // TODO: ダメージの表示
            }
            else
            {
                spellImage.sprite = null;
                spellDescription.text = "";
            }
        }
        get
        {
            return _spell;
        }
    }

    public delegate void OnClick(Spell? spell);
    public OnClick onClick
    {
        set
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => value(spell));
        }
    }


#pragma warning disable CS8618
    public TextMeshProUGUI spellDescription;
    public Image spellImage;
    public Button button;

#pragma warning restore CS8618
}
