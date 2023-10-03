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

    public bool GetIsMenuOpen { get => isMenuOpen; }
    public bool GetIsMsgHistoryOpen { get => isMsgHistoryOpen; }
    public bool GetIsInvOpen { get => isInvOpen; }
    public bool GetIsDropMenuOpen { get => isDropMenuOpen; }

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);
    }

    private void Start() => addMsg("Hi, welcome to da rl", "#0da2ff");

    public void setHealthMax(int maxHP)
    {
        hpSlider.maxValue = maxHP;
    }

    public void setHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHp}";
    }

    public void toggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            if (isMsgHistoryOpen)
                toggleMsgHistory();

            if (isInvOpen)
                toggleInv();

            if (isDropMenuOpen)
                toggleDropMenu();

            return;
        }
    }

    public void toggleMsgHistory()
    {
        msgHistory.SetActive(!msgHistory.activeSelf);
        isMsgHistoryOpen = msgHistory.activeSelf;
    }

    public void toggleInv(Actor actor = null)
    {
        inv.SetActive(!inv.activeSelf);
        isMenuOpen = inv.activeSelf;
        isInvOpen = inv.activeSelf;

        if (isMenuOpen)
            updateMenu(actor, invContent);
    }

    public void toggleDropMenu(Actor actor = null)
    {
        dropMenu.SetActive(!dropMenu.activeSelf);
        isMenuOpen = dropMenu.activeSelf;
        isDropMenuOpen = dropMenu.activeSelf;

        if (isMenuOpen)
            updateMenu(actor, dropMenuContent);
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
        for(int i =0; i < menuContent.transform.childCount; i++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(i).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            menuContentChild.GetComponent<Button>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        char c = 'a';
        for(int i = 0; i < actor.GetInventory.GetItems.Count; i++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(i).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) {actor.GetInventory.GetItems[i].name}";
            menuContentChild.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (menuContent == invContent)
                    Action.useAction(actor, i - 1);
                else if (menuContent == dropMenuContent)
                    Action.dropAction(actor, actor.GetInventory.GetItems[i - 1]);
                updateMenu(actor, menuContent);           
            });
            menuContentChild.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);

    }
}
