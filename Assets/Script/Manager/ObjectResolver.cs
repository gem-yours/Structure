using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectResolver : MonoBehaviour
{
    public static ObjectResolver instance;

    public delegate void OnResolveFinished(ObjectResolver resolver);

    public GameObject ui;
    public GameObject attack;
    public GameObject spell1;
    public GameObject spell2;
    public GameObject spell3;
    public GameObject unique;

    public TextMeshProUGUI levelText;
    public Slider expBar;

    public GameObject pickSpellWindow;
    public SpellCard spellCard1;
    public SpellCard spellCard2;
    public SpellCard spellCard3;
    public Button skipButton;

    public GameObject tile;
    public GameObject wall;
    public GameObject themisto;

    private static bool isResolveFinished = false;
    private static List<OnResolveFinished> listeners = new List<OnResolveFinished>();

    public static void AddListener(OnResolveFinished listener)
    {
        if (isResolveFinished)
        {
            listener(instance);
        }
        else
        {
            listeners.Add(listener);
        }
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

    private void ResolveUI()
    {
        ui = GameObject.Find("UI");
        attack = GameObject.Find("AttackButton");
        spell1 = GameObject.Find("Spell1Button");
        spell2 = GameObject.Find("Spell2Button");
        spell3 = GameObject.Find("Spell3Button");
        unique = GameObject.Find("UniqueButton");

        skipButton = GameObject.Find("SkipButton").GetComponent<Button>();

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        expBar = GameObject.Find("ExpBar").GetComponent<Slider>();

        pickSpellWindow = GameObject.Find("PickSpellWindow");
        spellCard1 = GameObject.Find("SpellCard1").GetComponent<SpellCard>();
        spellCard2 = GameObject.Find("SpellCard2").GetComponent<SpellCard>();
        spellCard3 = GameObject.Find("SpellCard3").GetComponent<SpellCard>();
    }

    private void ResolveResources()
    {
        tile = Resources.Load("Map/Tile") as GameObject;
        wall = Resources.Load("Map/Wall") as GameObject;
        themisto = Resources.Load("Characters/Themisto") as GameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResolveUI();
        ResolveResources();
        isResolveFinished = true;
        foreach (OnResolveFinished listener in listeners)
        {
            listener(this);
        }
        listeners.RemoveAll((OnResolveFinished x) => true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
