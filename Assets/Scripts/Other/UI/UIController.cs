using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UIController : UIPanelBase
{
    public static UIController Instance;

    
    public GameObject CardParent;

    
    public GuidePg GuidePg;

    
    public GameObject[] Cards;

    
    public GameObject[] CardsFinger;

    
    public Image WinPg;

    private Button WinBtn;

    
    public Image DeadPg;

    private Button LoseBtn;
    private int Level = 0;

    private bool isShowCard = false;

    private int _Tempindex = -1;

    private int _TempAppearIndex = -1;

    public GameObject CardObject;

    public bool isAppear = false;
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
            Instance = this;

        WinPg = GetControl<Image>("WinPg");
        DeadPg = GetControl<Image>("DeadPg");
        WinBtn = GetControl<Button>("WinBtn");
        LoseBtn = GetControl<Button>("LoseBtn");

        Restart();
    }

    public void ShowAndHideCardParent(bool isShow)
    {
        CardParent.SetActive(isShow);
        if (isShow && isShowCard)
            GameManager.Instance.SetStop(true);
    }

    public void AppearCardsFinger(bool isShow)
    {
        CardsFinger[Level].SetActive(isShow);
    }

    void Start()
    {
        WinBtn.onClick.AddListener(() =>
        {
            WX.NotifyMiniProgramPlayableStatus(new NotifyMiniProgramPlayableStatusOption()
            {
                isEnd = true,
            });
        });
        LoseBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.Restart();
        });
    }

    void Update()
    {
        if (Cards[0].activeSelf && Cards[1].activeSelf && !isAppear)
        {
            CardObject = Cards[0];
            CardObject.SetActive(false);
            isAppear = true;
        }
        if (CardObject != null && !Cards[1].activeSelf)
        {
            CardObject.SetActive(true);
            CardObject = null;
        }
    }

    public void AppearCard(int index)
    {
        Level = index;
        Cards[index].SetActive(true);
        CardsFinger[index].SetActive(true);
        isShowCard = true;
    }

    public void DisappearCard()
    {
        Cards[Level].SetActive(false);
        CardsFinger[Level].SetActive(false);
        GameManager.Instance.SetStop(false);
        isShowCard = false;
    }

    
    public void ShowEndPlayGround(bool iswin)
    {
        if (iswin)
        {
            WinPg.gameObject.SetActive(true);
            DeadPg.gameObject.SetActive(false);
            Cards[Level].SetActive(false);
            CardsFinger[Level].SetActive(false);
        }
        else
        {
            DeadPg.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        WinPg.gameObject.SetActive(false);
        DeadPg.gameObject.SetActive(false);
        if (Level != -1)
        {
            Cards[Level].SetActive(false);
            CardsFinger[Level].SetActive(false);
        }

    }
}
