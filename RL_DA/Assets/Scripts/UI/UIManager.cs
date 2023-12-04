using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager init;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private TextMeshProUGUI dungeonFloorText;

    [Header("Heath UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;

    [Header("Msg UI")]
    [SerializeField] private int sameMsgCount = 0; // Read only
    [SerializeField] private string lastMsg; // Read only
    [SerializeField] private bool isMsgHistoryOpen = false;
    [SerializeField] private GameObject msgHistory;
    [SerializeField] private GameObject msgHistoryContent;
    [SerializeField] private GameObject lastFiveMsgContent;

    [Header("Inv UI")]
    [SerializeField] private bool isInvOpen = false; // Read only
    [SerializeField] private GameObject inv;
    [SerializeField] private GameObject invContent;

    [Header("Drop menu UI")]
    [SerializeField] private bool isDropMenuOpen = false; // Read only
    [SerializeField] private GameObject dropMenu;
    [SerializeField] private GameObject dropMenuContent;

    [Header("Escape Menu UI")]
    [SerializeField] private bool isEscMenuOpen = false; // Read only
    [SerializeField] private GameObject escMenu;

    [Header("Character info menu UI")]
    [SerializeField] private bool isCharInfoOpen = false;
    [SerializeField] private GameObject charInfoMenu;

    [Header("Level up menu UI")]
    [SerializeField] private bool isLevelUpMenuOpen = false;
    [SerializeField] private GameObject levelUpMenu;
    [SerializeField] private GameObject levelUpMenuContent;

    public bool GetIsMenuOpen { get => isMenuOpen; }
    public bool GetIsMsgHistoryOpen { get => isMsgHistoryOpen; }
    public bool GetIsInvOpen { get => isInvOpen; }
    public bool GetIsDropMenuOpen { get => isDropMenuOpen; }
    public bool GetIsEscMenuOpen { get => isEscMenuOpen; }
    public bool GetIsCharMenuOpen { get => isCharInfoOpen; }

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        setDungeonFloorText(SaveManager.init.CurrentFloor);

        if(SaveManager.init.Save.SavedFloor is 0)
        {
             addMsg("Hi, welcome to da rl", "#0da2ff");
        }
        else
        {
            addMsg("Welcome back", "#0da2ff");
        }
    }


    public void setHealthMax(int maxHP)
    {
        hpSlider.maxValue = maxHP;
    }

    public void setHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHp}";
    }

    public void setDungeonFloorText(int floor)
    {
        dungeonFloorText.text = $"Dungeon floor: {floor}";
    }


    public void toggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            switch (true)
            {
                case bool _ when isMsgHistoryOpen:
                    toggleMsgHistory();
                    break;
                case bool _ when isInvOpen:
                    toggleInv();
                    break;
                case bool _ when isDropMenuOpen:
                    toggleDropMenu();
                    break;
                case bool _ when isEscMenuOpen:
                    toggleEscMenu();
                    break;
                case bool _ when isCharInfoOpen:
                    toggleCharInfoMenu();
                    break;
                default:
                    break;
            }
        }
    }

    public void toggleMsgHistory()
    {
        isMsgHistoryOpen = !isMsgHistoryOpen;
        setBools(msgHistory, isMsgHistoryOpen);
    }

    public void toggleInv(Actor actor = null)
    {
        isInvOpen = !isInvOpen;
        setBools(inv, isInvOpen);

        if (isMenuOpen)
            updateMenu(actor, invContent);
    }

    public void toggleDropMenu(Actor actor = null)
    {
        isDropMenuOpen = !isDropMenuOpen;
        setBools(dropMenu, isDropMenuOpen);

        if (isMenuOpen)
            updateMenu(actor, dropMenuContent);
    }

    public void toggleEscMenu()
    {
        isEscMenuOpen = !isEscMenuOpen;
        setBools(escMenu, isEscMenuOpen);

        eventSystem.SetSelectedGameObject(escMenu.transform.GetChild(0).gameObject);

    }

    public void toggleLevelUpMenu(Actor actor)
    {
        isLevelUpMenuOpen = !isLevelUpMenuOpen;
        setBools(levelUpMenu, isLevelUpMenuOpen);

        GameObject constitutionButton = levelUpMenuContent.transform.GetChild(0).gameObject;
        GameObject strengthButton = levelUpMenuContent.transform.GetChild(1).gameObject;
        GameObject agilityButton = levelUpMenuContent.transform.GetChild(2).gameObject;

        constitutionButton.GetComponent<TextMeshProUGUI>().text = $"a) Constitution (+20 HP, from {actor.GetComponent<Fighter>().MaxHp})";
        strengthButton.GetComponent<TextMeshProUGUI>().text = $"b) Strength (+1 attack, from {actor.GetComponent<Fighter>().Power})";
        agilityButton.GetComponent<TextMeshProUGUI>().text = $"c) Agility (+1 defense, from {actor.GetComponent<Fighter>().Defense})";

        foreach (Transform child in levelUpMenuContent.transform)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();

            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (constitutionButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncrMaxXP();
                }
                else if (strengthButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncrPower();
                }
                else if (agilityButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncrDefense();
                }
                else
                {
                    Debug.LogError("No buttons found!");
                }
                toggleLevelUpMenu(actor);
            });
        }

        eventSystem.SetSelectedGameObject(levelUpMenuContent.transform.GetChild(0).gameObject);
    }

    public void toggleCharInfoMenu(Actor actor = null)
    {
        isCharInfoOpen = !isCharInfoOpen;
        setBools(charInfoMenu, isCharInfoOpen);

        if(actor != null)
        {
            charInfoMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Level: {actor.GetComponent<Level>().CurrentLevel}";
            charInfoMenu.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"XP: {actor.GetComponent<Level>().CurrentXP}";
            charInfoMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"XP needed for next level: {actor.GetComponent<Level>().XpToNextLevel}";
            charInfoMenu.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Attack: {actor.GetComponent<Fighter>().Power}";
            charInfoMenu.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"Defense: {actor.GetComponent<Fighter>().Defense}";
        }
    }

    private void setBools(GameObject menu, bool menuBool)
    {
        isMenuOpen = menuBool;
        menu.SetActive(menuBool);
    }

    public void Save()
    {
        SaveManager.init.SaveGame(false);
        addMsg("The game has saved.", "#0da2ff");
    }

    public void Load()
    {
        SaveManager.init.LoadGame();
        addMsg("The game has loaded.", "#0da2ff");
        toggleMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }


    public void addMsg(string newMsg, string colorHex)
    {
        if (lastMsg == newMsg)
        {
            TextMeshProUGUI msgHistoryLastChild = msgHistoryContent.transform.GetChild(msgHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastFiveHistoryLastChild = lastFiveMsgContent.transform.GetChild(lastFiveMsgContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            msgHistoryLastChild.text = $"{newMsg} (x{sameMsgCount++})";
            lastFiveHistoryLastChild.text = $"{newMsg} (x{sameMsgCount})";
            return;
        }
        else if (sameMsgCount > 0)
            sameMsgCount = 0;

        lastMsg = newMsg;

        TextMeshProUGUI msgPref = Instantiate(Resources.Load<TextMeshProUGUI>("Msg")) as TextMeshProUGUI;
        msgPref.text = newMsg;
        msgPref.color = getColorFromHex(colorHex);
        msgPref.transform.SetParent(msgHistoryContent.transform, false);

        for(int i = 0; i < lastFiveMsgContent.transform.childCount; i++)
        {
            if(msgHistoryContent.transform.childCount - 1 < i)
            {
                return;
            }

            TextMeshProUGUI lastFiveHistoryChild = lastFiveMsgContent.transform.GetChild(lastFiveMsgContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI msgHistoryChild = msgHistoryContent.transform.GetChild(msgHistoryContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            lastFiveHistoryChild.text = msgHistoryChild.text;
            lastFiveHistoryChild.color = msgHistoryChild.color;
            
        }
    }

    private Color getColorFromHex(string v)
    {
        Color color;
        if(ColorUtility.TryParseHtmlString(v, out color))
        {
            return color;
        }
        else
        {
            Debug.Log("getColorFromHex: Could not get color from string");
            return Color.white;
        }
    }

    private void updateMenu(Actor actor, GameObject menuContent)
    {
        for(int resetNum = 0; resetNum < menuContent.transform.childCount; resetNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(resetNum).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            menuContentChild.GetComponent<Button>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        char c = 'a';
        for(int itemNum = 0; itemNum < actor.GetInventory.GetItems.Count; itemNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(itemNum).gameObject;
            Item item = actor.GetInventory.GetItems[itemNum];
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) {item.name}";
            menuContentChild.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (menuContent == invContent)
                    Action.useAction(actor, item);
                else if (menuContent == dropMenuContent)
                    Action.dropAction(actor, item);      
            });
            menuContentChild.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);

    }
}
