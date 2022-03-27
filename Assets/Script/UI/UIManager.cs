using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public delegate void OnCast(SpellSlot spellSlot);


    public GameObject ui;
    public Button attackButton;
    public Button spell1Button;
    public Button spell2Button;
    public Button spell3Button;
    public Button uniqueButton;

    public TextMeshProUGUI levelText;
    public Slider expBar;

    public GameObject pickSpellWindow;
    public SpellCard spellCard1;
    public SpellCard spellCard2;
    public SpellCard spellCard3;
    public Button skipButton;

    public DeckPreview deckPreview;

    public Deck deck
    {
        set
        {
            deckPreview.deck = value;
        }
    }

    public OnCast onCast
    {
        set
        {
            attackButton.onClick.AddListener(() => value(SpellSlot.Attack));
            spell1Button.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell1));
            spell2Button.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell2));
            spell3Button.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Spell3));
            uniqueButton.GetComponent<Button>().onClick.AddListener(() => value(SpellSlot.Unique));
        }
    }

    public int level
    {
        set
        {
            levelText.text = value.ToString();
        }
    }
    public int requireExp
    {
        set
        {
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

    public bool isUiActive
    {
        get
        {
            return ui.activeSelf;
        }
        set
        {
            ui.SetActive(value);
        }
    }


    public void ShowPickSpellWindow(Spell spell1, Spell spell2, Spell spell3, SpellCard.OnClick onSpellPicked)
    {
        spellCard1.spell = spell1;
        spellCard1.onClick = (Spell spell) => onSpellPicked(spell);
        spellCard2.spell = spell2;
        spellCard2.onClick = onSpellPicked;
        spellCard3.spell = spell3;
        spellCard3.onClick = onSpellPicked;


        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() => onSpellPicked(null));

        pickSpellWindow.SetActive(true);
    }

    public void HidePickSpellWindow()
    {
        pickSpellWindow.SetActive(false);
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
        pickSpellWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
