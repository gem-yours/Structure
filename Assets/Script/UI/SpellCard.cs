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
            image = Resources.Load<Sprite>("SpellIcon/" + value.imageName);
            description = value.description;
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
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => value(spell));
        }
    }

    private Sprite image
    {
        set
        {
            spellImage.sprite = value;
        }
    }

    private string description
    {
        set
        {
            spellDescription.text = value;
        }
    }

    private Button button;
    private Image spellImage;
    private TextMeshProUGUI spellDescription;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        spellImage = GameObject.Find("SpellImage").GetComponent<Image>();
        spellDescription = GameObject.Find("SpellDescription").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
