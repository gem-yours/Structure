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
    public delegate void OnSpellPushed(SpellSlot spellSlot);

    public GameObject? ui;
#pragma warning disable CS8618
    public SpellSlotController attackController;
    public SpellSlotController spell1Controller;
    public SpellSlotController spell2Controller;
    public SpellSlotController spell3Controller;

    public GameObject gameOverWindow;
    public Button exitToTileButton;
#pragma warning restore CS8618

    public TextMeshProUGUI? levelText;

    public GameObject? pickSpellWindow;
    public SpellCard? spellCard1;
    public SpellCard? spellCard2;
    public SpellCard? spellCard3;
    public Button? skipButton;

#pragma warning disable CS8618
    public Slider expBar;
    public Slider hpBar;
    public MiniMap miniMap;
    public MiniMap worldMap;
    public Button menuButton;
    public GameObject menu;
    public DeckContents deckContents;
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

            attackController.dragController.onClick = () => value();
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

    public OnSpellPushed onSpellPushed
    {
        set
        {
            spell1Controller.dragController.onPushed = () =>
            {
                value(SpellSlot.Spell1);
            };
            spell2Controller.dragController.onPushed = () =>
            {
                value(SpellSlot.Spell2);
            };
            spell3Controller.dragController.onPushed = () =>
            {
                value(SpellSlot.Spell3);
            };
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
            expBar.value = value;
        }
    }

    public float maxHp
    {
        set
        {
            hpBar.maxValue = value;
        }
    }

    public float currentHp
    {
        set
        {
            hpBar.value = value;
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

    public void ShowMenu(Deck deck, UnityEngine.Events.UnityAction onClick)
    {
        deckContents.deck = deck;
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
        spellCard2!.spell = spell2;
        spellCard3!.spell = spell3;
        pickSpellWindow!.SetActive(true);
        StartCoroutine(SetPickingSpellAction(onSpellPicked));
    }

    // メニューが表示されてすぐに操作できると操作が事故るので、短い間操作不能にする
    private IEnumerator SetPickingSpellAction(SpellCard.OnClick onSpellPicked)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        // TODO: アニメーションを追加する
        spellCard1!.onClick = (Spell? spell) => onSpellPicked(spell);
        spellCard2!.onClick = onSpellPicked;
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
        var dragController = GetControllerBySlot((SpellSlot)slot).dragController;
        if (dragController == null) return;

        icon.AttachTo(dragController.gameObject);
    }


    public void SetAutoAttack(Spell spell)
    {
        var iconObject = Instantiate(Resources.Load("SpellIcon/SpellIcon"), Vector3.zero, Quaternion.identity, transform) as GameObject;
        if (iconObject is null) return;
        var spellIcon = iconObject.GetComponent<SpellIcon>();
        if (spellIcon is null) return;
        spellIcon.spell = spell;
        spellIcon.AttachTo(attackController.dragController.gameObject, 0);
    }

    public void UnsetSpell(SpellSlot slot)
    {
        var button = GetControllerBySlot(slot).dragController;
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

    public SpellSlotController GetControllerBySlot(SpellSlot slot)
    {
        return slot switch
        {
            SpellSlot.Spell1 => spell1Controller,
            SpellSlot.Spell2 => spell2Controller,
            SpellSlot.Spell3 => spell3Controller,
            _ => throw new System.Exception("SpellSlot should not be null")
        };
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
}
