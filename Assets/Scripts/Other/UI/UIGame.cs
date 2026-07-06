using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIPanelBase
{
    public static UIGame Instance;
    [Header("改变装备和武器的图案")]
    public Sprite NewWeaponIcon;
    public Sprite NewEquipIcon;

    private Image WeaponIcon;

    private Image EquipIcon;

    private Image TouchArea;

    private Image CoinIcon;

    //-------增加经验条的控件-------
    private Image ExperienceBar;

    private TextMeshProUGUI ExperienceLv;

    private TextMeshProUGUI ExperienceNowNumber;

    private TextMeshProUGUI ExperienceMaxNumber;

    private int ExperienceLvNumber_int = 0;

    private int ExperienceMaxNumber_int = 0;

    private int ExperienceNowNumber_int = 0;

    private int AddExperienceCount = 0;

    private int TempAddExperienceCount = 0;

    private float ExperienceAddCoolTimer = 0.3f;

    private float ExperienceAddimer = 0f;

    //-------增加金币的控件-------

    private TextMeshProUGUI CoinNumber;

    private TextMeshProUGUI AddCoinNumber;

    private Animator AddCoinNumberAnimator;

    private bool isFirstAddCoinPlayAnimator = false;

    private int CoinNumber_int = 0;

    private int AddCoinCount = 0;

    private float CoinAddCoolTimer = 0.3f;

    private float CoinAddimer = 0f;

    
    private Image PowerUp;

    private TextMeshProUGUI UpText;

    private TextMeshProUGUI PowerText;

    private int UpText_int = 0;

    private int PowerText_int = 0;

    private float PowerAddCoolTimer = 0.3f;

    private float PowerAddimer = 0f;

    
    public GameObject GamePg;

    
    public GameObject EquipPg;

    
    private Image DangerEF;

    
    private Image PlayGoundTipTitle;

    

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        WeaponIcon = GetControl<Image>("WeaponIcon");

        EquipIcon = GetControl<Image>("EquipIcon");

        TouchArea = GetControl<Image>("TouchArea");

        CoinIcon = GetControl<Image>("CoinIcon");

        ExperienceLv = GetControl<TextMeshProUGUI>("ExperienceLv");

        ExperienceNowNumber = GetControl<TextMeshProUGUI>("ExperienceNowNumber");

        ExperienceMaxNumber = GetControl<TextMeshProUGUI>("ExperienceMaxNumber");

        ExperienceBar = GetControl<Image>("ExperienceBar");

        CoinNumber = GetControl<TextMeshProUGUI>("CoinNumber");

        AddCoinNumber = GetControl<TextMeshProUGUI>("AddCoinNumber");

        AddCoinNumberAnimator = AddCoinNumber.GetComponent<Animator>();

        PowerUp = GetControl<Image>("PowerUp");

        UpText = GetControl<TextMeshProUGUI>("UpText");

        PowerText = GetControl<TextMeshProUGUI>("PowerText");

        DangerEF = GetControl<Image>("DangerEF");

        PlayGoundTipTitle = GetControl<Image>("PlayGoundTipTitle");

        
    }

    void Start()
    {
        

        ExperienceLvNumber_int = int.Parse(ExperienceLv.text);
        ExperienceMaxNumber_int = int.Parse(ExperienceMaxNumber.text);
        ExperienceNowNumber_int = int.Parse(ExperienceNowNumber.text);

        ExperienceLv.text = Tools.NumToArtNum("L" + ExperienceLvNumber_int.ToString());
        ExperienceMaxNumber.text = Tools.NumToArtNum(ExperienceMaxNumber_int.ToString());
        ExperienceNowNumber.text = Tools.NumToArtNum(ExperienceNowNumber_int.ToString());

        CoinNumber_int = int.Parse(CoinNumber.text);
        AddCoinCount = CoinNumber_int;
        CoinNumber.text = Tools.NumToArtNum(CoinNumber_int.ToString());

        PowerText_int = int.Parse(PowerText.text);
        UpText_int = int.Parse(UpText.text);
        UpText.text = Tools.NumToArtNum(UpText_int.ToString());
        PowerText.text = Tools.NumToArtNum("0" + PowerText_int.ToString());
        HidePowerUp();
        AddCoinNumber.gameObject.SetActive(false);
        DangerEF.gameObject.SetActive(false);
        PlayGoundTipTitle.gameObject.SetActive(false);
    }

    public void AddExperience(int experience)
    {
        
        ExperienceNowNumber_int += experience;
        while (ExperienceNowNumber_int > ExperienceMaxNumber_int)
        {
            ExperienceLvNumber_int += 1;
            ExperienceNowNumber_int -= ExperienceMaxNumber_int;
            ExperienceMaxNumber_int += Random.Range(200, 400);
            ExperienceLv.text = Tools.NumToArtNum("L" + ExperienceLvNumber_int.ToString());
        }
        ExperienceBar.fillAmount = (float)ExperienceNowNumber_int / (float)ExperienceMaxNumber_int;
        ExperienceMaxNumber.text = Tools.NumToArtNum(ExperienceMaxNumber_int.ToString());
        ExperienceNowNumber.text = Tools.NumToArtNum(ExperienceNowNumber_int.ToString());
    }

    public void ClearAddExperience()
    {
        AddExperienceCount = 0;
        TempAddExperienceCount = 0;
        AddExperienceCount = 0;
    }

    public void AddCoin(int coin)
    {
        AddCoinNumber.gameObject.SetActive(true);
        AddCoinNumber.text = Tools.NumToArtNum(coin.ToString());
        AddCoinCount += coin;
        if (!isFirstAddCoinPlayAnimator)
        {
            isFirstAddCoinPlayAnimator = true;
            AddCoinNumberAnimator.SetTrigger("isAdd");
        }
    }

    public void AddPower(int power)
    {
        PowerUp.gameObject.SetActive(true);
        UpText_int = power;
        UpText.text = Tools.NumToArtNum(UpText_int.ToString());
        AudioManager.Instance.PlaySound("UpPower");
        UpText_int = PowerText_int + power;

        
        PowerText_int = PowerText_int + power;
        PowerText.text = Tools.NumToArtNum("0" + PowerText_int.ToString());
        Invoke("HidePowerUp", 4f);
    }

    public void HidePowerUp()
    {
        PowerUp.gameObject.SetActive(false);
    }

    void Update()
    {
        
        Coin();
        
    }

    private void Experience()
    {
        if (AddExperienceCount > 0)
        {
            if (TempAddExperienceCount != AddExperienceCount)
            {
                if (ExperienceAddimer < ExperienceAddCoolTimer)
                {
                    ExperienceAddimer += Time.deltaTime * 100f;
                    if (ExperienceAddimer >= ExperienceAddCoolTimer)
                    {
                        ExperienceNowNumber_int += 1;
                        TempAddExperienceCount += 1;
                        ExperienceBar.fillAmount = (float)ExperienceNowNumber_int / ExperienceMaxNumber_int;
                        
                        if (ExperienceNowNumber_int >= ExperienceMaxNumber_int)
                        {
                            ExperienceNowNumber_int = 0;
                            ExperienceLvNumber_int += 1;
                            ExperienceLv.text = Tools.NumToArtNum("L" + ExperienceLvNumber_int.ToString());
                            ExperienceMaxNumber_int += Random.Range(200, 400);
                            ExperienceMaxNumber.text = Tools.NumToArtNum(ExperienceMaxNumber_int.ToString());
                            ExperienceBar.fillAmount = 0;
                        }
                        ExperienceNowNumber.text = Tools.NumToArtNum(ExperienceNowNumber_int.ToString());
                        ExperienceAddimer = 0f;
                    }
                }
            }
            else
            {
                ExperienceAddimer = 0f;
                TempAddExperienceCount = 0;
                AddExperienceCount = 0;
            }
        }
    }

    private void Coin()
    {
        if (AddCoinCount > 0)
        {
            if (CoinNumber_int != AddCoinCount)
            {
                if (CoinAddimer < CoinAddCoolTimer)
                {
                    CoinAddimer += Time.deltaTime * 10f;
                    if (CoinAddimer >= CoinAddCoolTimer)
                    {
                        CoinNumber_int += 1;
                        CoinNumber.text = Tools.NumToArtNum(CoinNumber_int.ToString());
                        CoinAddimer = 0f;
                    }
                }
            }
            else
            {
                CoinAddimer = 0f;
                isFirstAddCoinPlayAnimator = false;
            }
        }
    }

    private void Power()
    {
        if (UpText_int > 0)
        {
            if (PowerText_int != UpText_int)
            {
                if (PowerAddimer < PowerAddCoolTimer)
                {
                    PowerAddimer += Time.deltaTime * 100f;
                    if (PowerAddimer >= PowerAddCoolTimer)
                    {
                        PowerText_int += 1;
                        PowerText.text = Tools.NumToArtNum("0" + PowerText_int.ToString());
                        PowerAddimer = 0f;
                    }
                }
            }
            else
            {
                PowerAddimer = 0f;
                UpText_int = 0;
                Invoke("HidePowerUp", 2f);
            }
        }
    }

    public RectTransform GetCoinIconRect()
    {
        return CoinIcon.GetComponent<RectTransform>();
    }

    public void RestartClearjoystickFingerId()
    {
        TouchArea.GetComponent<TouchArea>().ClearjoystickFingerId();
    }

    public void StopTouchArea()
    {
        TouchArea.GetComponent<TouchArea>().NewJoystick.Hide();
    }

    public void ShowTouchArea(bool isTouch)
    {
        if (isTouch)
            TouchArea.gameObject.SetActive(true);
        else
            TouchArea.gameObject.SetActive(false);
    }

    public void ShowNewWeapon()
    {
        WeaponIcon.sprite = NewWeaponIcon;
        WeaponIcon.color = new Color(1, 1, 1, 1);
    }

    public void ShowNewEquip()
    {
        EquipIcon.sprite = NewEquipIcon;
        EquipIcon.color = new Color(1, 1, 1, 1);
    }

    public void ShowGameAndEquipPg(bool isShow)
    {
        if (isShow)
        {
            GamePg.SetActive(true);
            EquipPg.SetActive(true);
        }
        else
        {
            GamePg.SetActive(false);
            EquipPg.SetActive(false);
        }
    }

    public void ShowDangerEF()
    {
        DangerEF.gameObject.SetActive(true);
        DangerEF.GetComponent<Animator>().SetTrigger("isDangerEF");
        Invoke("HideDangerEF", 0.5f);
    }

    private void HideDangerEF()
    {
        DangerEF.gameObject.SetActive(false);
    }

    

    public void ShowAndHidePlayGoundTipTitle(bool isShow)
    {
        PlayGoundTipTitle.gameObject.SetActive(isShow);
    }
}
