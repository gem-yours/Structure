using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#nullable enable
public class UIManager : MonoBehaviour
{
#pragma warning disable CS8618
    public static UIManager instance;
#pragma warning restore CS8618
    public delegate void OnCast(SpellSlot spellSlot);


    public GameObject? ui;
    public DragController? attackController;
    public DragController? spell1Controller;
    public DragController? spell2Controller;
    public DragController? spell3Controller;

    public TextMeshProUGUI? levelText;
    public Slider? expBar;

    public GameObject? pickSpellWindow;
    public SpellCard? spellCard1;
    public SpellCard? spellCard2;
    public SpellCard? spellCard3;
    public Button? skipButton;

    public DeckPreview? deckPreview;

    public DragController? dragController;

    public Deck deck
    {
        set
        {
            if (deckPreview == null) return;
            deckPreview.deck = value;
        }
    }

    public OnCast onCast
    {
        set
        {
            if (spell1Controller != null)
                spell1Controller.onClick = () => value(SpellSlot.Spell1);
            if (spell2Controller != null)
                spell2Controller.onClick = () => value(SpellSlot.Spell2);
            if (spell3Controller != null)
                spell3Controller.onClick = () => value(SpellSlot.Spell3);
        }
    }

    public int level
    {
        set
        {
            if (levelText == null) return;
            levelText.text = value.ToString();
        }
    }
    public int requireExp
    {
        set
        {
            if (expBar == null) return;
            expBar.maxValue = value;
        }
    }
    public int exp
    {
        set
        {
            if (expBar == null) return;
            expBar.value = value;
        }
    }

    public bool isUiActive
    {
        get
        {
            if (ui == null) return false;
            return ui.GetComponent<Canvas>().enabled;
        }
        set
        {
            if (!value)
            {
                dragController?.ForceEndDrag();
            }
            if (ui == null) return;
            ui.GetComponent<Canvas>().enabled = value;
        }
    }


    public void ShowPickSpellWindow(Spell spell1, Spell spell2, Spell spell3, SpellCard.OnClick onSpellPicked)
    {
        spellCard1!.spell = spell1;
        spellCard1!.onClick = (Spell spell) => onSpellPicked(spell);
        spellCard2!.spell = spell2;
        spellCard2!.onClick = onSpellPicked;
        spellCard3!.spell = spell3;
        spellCard3!.onClick = onSpellPicked;


        skipButton!.onClick.RemoveAllListeners();
        skipButton!.onClick.AddListener(() => onSpellPicked(null));

        pickSpellWindow!.SetActive(true);
    }

    public void HidePickSpellWindow()
    {
        pickSpellWindow!.SetActive(false);
    }

    public void SetSpell(SpellIcon icon)
    {
        var slot = GameManager.instance.player.slots.GetEmptySlot();
        if (slot == null) return;

        var button = GetButtonBySlot((SpellSlot)slot);
        if (button == null) return;

        icon.transform.SetParent(button.transform);
        icon.AttachTo(button.gameObject);
        // icon.transform.position = button.transform.position;
    }

    public void UnsetSpell(SpellSlot slot)
    {
        var button = GetButtonBySlot(slot);
        if (button == null) return;

        for (var i = 0; i < button.transform.childCount; i++)
        {
            var child = button.transform.GetChild(i).gameObject;
            if (child != null && child.GetComponent<SpellIcon>() != null)
            {
                Destroy(child);
                break;
            }
        }
    }

    private DragController? GetButtonBySlot(SpellSlot slot)
    {
        return slot switch
        {
            SpellSlot.Spell1 => spell1Controller,
            SpellSlot.Spell2 => spell2Controller,
            SpellSlot.Spell3 => spell3Controller,
            _ => throw new System.Exception("SpellSlot should not be null.")
        };
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
        pickSpellWindow!.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
