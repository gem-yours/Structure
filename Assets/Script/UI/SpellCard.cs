using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SpellCard : MonoBehaviour
{
    private Spell _spell;
    public Spell spell
    {
        set
        {
            _spell = value;

            spellImage.sprite = spell.image;
            spellDescription.text = value.description;
            // TODO: ダメージの表示
        }
        get
        {
            return _spell;
        }
    }

    public delegate void OnClick(Spell spell);
    public OnClick onClick
    {
        set
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => value(spell));
        }
    }

    private Button button;
    public Image spellImage;
    public TextMeshProUGUI spellDescription;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
