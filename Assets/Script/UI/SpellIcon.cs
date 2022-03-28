using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellIcon : MonoBehaviour
{
    public Image spellImage;

    private Spell _spell;
    public Spell spell
    {
        set
        {
            _spell = value;

            spellImage.sprite = Resources.Load<Sprite>("SpellIcon/" + value.imageName);
        }
        get
        {
            return _spell;
        }
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
