using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public delegate void OnCast(SpellSlot spellSlot);

    public OnCast onCast
    {
        set
        {
            ObjectResolver.instance.attack.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Attack));
            ObjectResolver.instance.spell1.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell1));
            ObjectResolver.instance.spell2.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell2));
            ObjectResolver.instance.spell3.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell3));
            ObjectResolver.instance.unique.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Unique));
        }
    }

    public int level
    {
        set
        {
            ObjectResolver.instance.levelText.text = value.ToString();
        }
    }
    public int requireExp
    {
        set
        {
            ObjectResolver.instance.expBar.maxValue = value;
        }
    }
    public int exp
    {
        set
        {
            ObjectResolver.instance.expBar.value = value;
        }
    }

    public bool isUiActive
    {
        get
        {
            return ObjectResolver.instance.ui.activeSelf;
        }
        set
        {
            ObjectResolver.instance.ui.SetActive(value);
        }
    }

    public void ShowPickSpellWindow(Spell spell1, Spell spell2, Spell spell3, SpellCard.OnClick onSpellPicked)
    {
        ObjectResolver.instance.pickSpellWindow.SetActive(true);

        ObjectResolver.instance.spellCard1.spell = spell1;
        ObjectResolver.instance.spellCard1.onClick = onSpellPicked;
        ObjectResolver.instance.spellCard2.spell = spell2;
        ObjectResolver.instance.spellCard2.onClick = onSpellPicked;
        ObjectResolver.instance.spellCard3.spell = spell3;
        ObjectResolver.instance.spellCard3.onClick = onSpellPicked;

        ObjectResolver.instance.skipButton.onClick.RemoveAllListeners();
        ObjectResolver.instance.skipButton.onClick.AddListener(() => onSpellPicked(null));
    }

    public void HidePickSpellWindow()
    {
        ObjectResolver.instance.pickSpellWindow.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjectResolver.AddListener((resolver) =>
        {
            resolver.pickSpellWindow.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
