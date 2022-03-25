using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellCard : MonoBehaviour
{
    public Spell spell
    {
        set
        {
            image = value.image;
            description = value.description;
            // TODO: ダメージの表示
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

    private Image spellImage;
    private TextMeshProUGUI spellDescription;
    // Start is called before the first frame update
    void Start()
    {
        spellImage = GameObject.Find("SpellImage").GetComponent<Image>();
        spellDescription = GameObject.Find("SpellDescription").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
