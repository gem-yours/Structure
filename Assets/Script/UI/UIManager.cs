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
            attackButton.onClick.AddListener(() => value(SpellSlot.Attack));
            spell1Button.onClick.AddListener(() => value(SpellSlot.Spell1));
            spell2Button.onClick.AddListener(() => value(SpellSlot.Spell2));
            spell3Button.onClick.AddListener(() => value(SpellSlot.Spell3));
            uniqueButton.onClick.AddListener(() => value(SpellSlot.Unique));
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
            return root.activeSelf;
        }
        set
        {
            root.SetActive(value);
        }
    }

    private GameObject root;
    private Button attackButton;
    private Button spell1Button;
    private Button spell2Button;
    private Button spell3Button;
    private Button uniqueButton;

    private TextMeshProUGUI levelText;
    private Slider expBar;

    private GameObject pickSpellWindow;
    private SpellCard spellCard1;
    private SpellCard spellCard2;
    private SpellCard spellCard3;
    private Button skipButton;


    public void ShowPickSpellWindow(Spell spell1, Spell spell2, Spell spell3, SpellCard.OnClick onSpellPicked)
    {
        pickSpellWindow.SetActive(true);

        spellCard1.spell = spell1;
        spellCard1.onClick = onSpellPicked;
        spellCard2.spell = spell2;
        spellCard2.onClick = onSpellPicked;
        spellCard3.spell = spell3;
        spellCard3.onClick = onSpellPicked;

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() => onSpellPicked(null));
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

    private void Init()
    {
        root = GameObject.Find("UI");
        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        spell1Button = GameObject.Find("Spell1Button").GetComponent<Button>();
        spell2Button = GameObject.Find("Spell2Button").GetComponent<Button>();
        spell3Button = GameObject.Find("Spell3Button").GetComponent<Button>();
        uniqueButton = GameObject.Find("UniqueButton").GetComponent<Button>();

        skipButton = GameObject.Find("SkipButton").GetComponent<Button>();

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        expBar = GameObject.Find("ExpBar").GetComponent<Slider>();

        pickSpellWindow = GameObject.Find("PickSpellWindow");
        spellCard1 = GameObject.Find("SpellCard1").GetComponent<SpellCard>();
        spellCard2 = GameObject.Find("SpellCard2").GetComponent<SpellCard>();
        spellCard3 = GameObject.Find("SpellCard3").GetComponent<SpellCard>();
        pickSpellWindow.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
