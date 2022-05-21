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
#pragma warning disable CS8618
    public SpellSlotController spell1Controller;
    public SpellSlotController spell2Controller;
    public SpellSlotController spell3Controller;
#pragma warning restore CS8618

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
            spell1Controller.dragController.onDragging = (Vector2 displacement) => value(SpellSlot.Spell1, displacement);
            spell2Controller.dragController.onDragging = (Vector2 displacement) => value(SpellSlot.Spell2, displacement);
            spell3Controller.dragController.onDragging = (Vector2 displacement) => value(SpellSlot.Spell3, displacement);
        }
    }

    public OnSpellAction onEndDragging
    {
        set
        {
            spell1Controller.dragController.onEndDragging = () => value(SpellSlot.Spell1);
            spell2Controller.dragController.onEndDragging = () => value(SpellSlot.Spell2);
            spell3Controller.dragController.onEndDragging = () => value(SpellSlot.Spell3);
        }
    }

    public OnSpellAction onClickSpell
    {
        set
        {
            spell1Controller.dragController.onClick = () => value(SpellSlot.Spell1);
            spell2Controller.dragController.onClick = () => value(SpellSlot.Spell2);
            spell3Controller.dragController.onClick = () => value(SpellSlot.Spell3);
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
        pickSpellWindow!.SetActive(true);
        StartCoroutine(SetPickAction(spell1, spell2, spell3, onSpellPicked));
    }

    // メニューが表示されてすぐに操作できると操作が事故るので、短い間操作不能にする
    private IEnumerator SetPickAction(Spell spell1, Spell spell2, Spell spell3, SpellCard.OnClick onSpellPicked)
    {
        yield return new WaitForSeconds(0.5f);
        spellCard1!.spell = spell1;
        spellCard1!.onClick = (Spell? spell) => onSpellPicked(spell);
        spellCard2!.spell = spell2;
        spellCard2!.onClick = onSpellPicked;
        spellCard3!.spell = spell3;
        spellCard3!.onClick = onSpellPicked;


        skipButton!.onClick.RemoveAllListeners();
        skipButton?.onClick.AddListener(() => onSpellPicked(null));
    }

    public void HidePickSpellWindow()
    {
        pickSpellWindow!.SetActive(false);
    }

    public void SetSpell(SpellSlot slot, SpellIcon icon)
    {
        var button = GetDragControllerBySlot((SpellSlot)slot);
        if (button == null) return;

        icon.transform.SetParent(button.transform);
        icon.AttachTo(button.gameObject);
    }

    public void UnsetSpell(SpellSlot slot)
    {
        var button = GetDragControllerBySlot(slot);
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

    private DragController? GetDragControllerBySlot(SpellSlot slot)
    {
        return slot switch
        {
            SpellSlot.Spell1 => spell1Controller.dragController,
            SpellSlot.Spell2 => spell2Controller.dragController,
            SpellSlot.Spell3 => spell3Controller.dragController,
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
