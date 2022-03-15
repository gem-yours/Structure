using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private Button battleButton;
    private Button equipButton;
    private GameObject battleScreen;
    private GameObject equipScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance == this)
        {
            Destroy(gameObject);
        }
    }


    private void Init() 
    {
        battleButton = GameObject.Find("BattleButton").GetComponent<Button>();
        equipButton = GameObject.Find("EquipButton").GetComponent<Button>();
        battleScreen = GameObject.Find("BattleScreen");
        equipScreen = GameObject.Find("EquipScreen");

        
        battleButton.onClick.AddListener(() => ActivateScreen(battleScreen));
        equipButton.onClick.AddListener(() => ActivateScreen(equipScreen));
        battleButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));

        ActivateScreen(battleScreen);
    }

    private void ActivateScreen(GameObject screen)
    {
        battleScreen.SetActive(false);
        equipScreen.SetActive(false);

        screen.SetActive(true);
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
