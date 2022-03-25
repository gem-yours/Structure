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

    public bool active
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
        root = GameObject.Find("UI");
        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        spell1Button = GameObject.Find("Spell1Button").GetComponent<Button>();
        spell2Button = GameObject.Find("Spell2Button").GetComponent<Button>();
        spell3Button = GameObject.Find("Spell3Button").GetComponent<Button>();
        uniqueButton = GameObject.Find("UniqueButton").GetComponent<Button>();

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        expBar = GameObject.Find("ExpBar").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
