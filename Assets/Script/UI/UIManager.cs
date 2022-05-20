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

    public delegate void OnAttack();
    public delegate void OnSpellAction(SpellSlot spellSlot);
    public delegate void OnSpellDragging(SpellSlot spellSlot, Vector2 displacement);

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

#pragma warning disable CS8618
    public MiniMap miniMap;
    public MiniMap worldMap;
    public Button menuButton;
    public GameObject menu;
    public Button returnFromMenu;
    public GameObject loadingScreen;
    public TextMeshProUGUI loadingText;
#pragma warning restore CS8618

    public DeckPreview? deckPreview;

    public DragController? movingController;

    public Deck deck
    {
        set
        {
            if (deckPreview == null) return;
            deckPreview.deck = value;
        }
    }

    public OnAttack onAttack
    {
        set
        {
            if (attackController != null)
            {
                attackController.onClick = () => value();
            }
        }
    }

    public OnSpellDragging onDraggingSpell
    {
        set
        {
            if (spell1Controller != null)
                spell1Controller.onDragging = (Vector2 displacement) => value(SpellSlot.Spell1, displacement);
            if (spell2Controller != null)
                spell2Controller.onDragging = (Vector2 displacement) => value(SpellSlot.Spell2, displacement);
            if (spell3Controller != null)
                spell3Controller.onDragging = (Vector2 displacement) => value(SpellSlot.Spell3, displacement);
        }
    }

    public OnSpellAction onEndDragging
    {
        set
        {
            if (spell1Controller != null)
                spell1Controller.onEndDragging = () => value(SpellSlot.Spell1);
            if (spell2Controller != null)
                spell2Controller.onEndDragging = () => value(SpellSlot.Spell2);
            if (spell3Controller != null)
                spell3Controller.onEndDragging = () => value(SpellSlot.Spell3);
        }
    }

    public OnSpellAction onClickSpell
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
                movingController?.ForceEndDrag();
            }
            if (ui == null) return;
            ui.GetComponent<Canvas>().enabled = value;
        }
    }

    public void ShowMenu(UnityEngine.Events.UnityAction onClick)
    {
        returnFromMenu.onClick.AddListener(onClick);
        menu.SetActive(true);
    }

    public void HideMenu()
    {
        returnFromMenu.onClick.RemoveAllListeners();
        menu.SetActive(false);
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

    public void SetSpell(SpellSlot slot, SpellIcon icon)
    {
        var button = GetButtonBySlot((SpellSlot)slot);
        if (button == null) return;

        icon.transform.SetParent(button.transform);
        icon.AttachTo(button.gameObject);
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
                Destroy(child.gameObject);
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
