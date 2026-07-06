using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LifeLookCamera : UIPanelBase
{
    
    public TMP_SpriteAsset NumberTextSpriteAsset;

    
    public TMP_SpriteAsset RedNumberTextSpriteAsset;

    
    public TMP_SpriteAsset BlueNumberTextSpriteAsset;

    
    private Image LifeFill;

    private Image AttackFill;

    private Image AttackSlider;

    
    private Image Fill;

    private TextMeshProUGUI LifeNumber;

    public GameObject HurtNumberTextPrefab;

    private Camera cam;
    protected override void Awake()
    {
        base.Awake();
        Fill = GetControl<Image>("Fill");
        LifeFill = GetControl<Image>("LifeFill");
        AttackFill = GetControl<Image>("AttackFill");
        LifeNumber = GetControl<TextMeshProUGUI>("LifeNumber");
        AttackSlider = GetControl<Image>("AttackSlider");
    }

    public void SetLife(int value)
    {
        LifeNumber.text = Tools.NumToArtNum(value.ToString());
    }

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    
    void Update()
    {
        
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                         cam.transform.rotation * Vector3.up);
        

    }

    
    public void AppearHurtNumber(int _value, int Number, int type)
    {
        int value = _value;
        if (Number > 1 && Number != 5)
            value = (int)(_value / Number);
        else if (Number == 5)
            value = (int)(_value / Number);
        else
            Number = 1;
        StartCoroutine(HurtNumber(value, Number, type));
    }

    IEnumerator HurtNumber(int value, int Number, int type)
    {
        float _Scale = 1.2f;
        float _OldScale = 0.8f;
        float _MoveY = 1.2f;
        float _OldMoveY = -0.5f;
        if (type != 0)
        {
            _Scale = 1.5f;
            _OldScale = 1.1f;
            _MoveY = 0.6f;
            _OldMoveY = -0.6f;
        }

        if (Number == 1)
        {
            _Scale = 1.5f;
            _OldScale = 1.3f;
            _MoveY = 0f;
            _OldMoveY = -0.5f;
            if (type != 0)
            {
                _Scale = 1.8f;
                _OldScale = 1.6f;
                _MoveY = -0.5f;
                _OldMoveY = -0.8f;
            }

        }
        else if (Number == 5)
        {
            _Scale = 1.3f;
            _OldScale = 1.1f;
            _MoveY = 0.5f;
            _OldMoveY = -0.3f;
            if (type != 0)
            {
                _Scale = 1.6f;
                _OldScale = 1.4f;
                _MoveY = 0f;
                _OldMoveY = -0.4f;
            }

        }

        for (int i = 1; i <= Number; i++)
        {
            Transform HurtNumberText_tran = Instantiate(HurtNumberTextPrefab, transform.GetChild(0), false).transform;
            HurtNumberText_tran.position += new Vector3(Random.Range(-0.4f, 0.41f), Random.Range(-0.6f, 0.6f), 0);
            HurtNumberText_tran.parent = transform;
            if (Number == 5)
                HurtNumberText_tran.position -= new Vector3(0, 0.2f, 0);
            TextMeshProUGUI HurtNumberText = HurtNumberText_tran.GetComponent<TextMeshProUGUI>();
            
            if (type == 0)
                HurtNumberText.spriteAsset = NumberTextSpriteAsset;
            else if (type == 1)
                HurtNumberText.spriteAsset = RedNumberTextSpriteAsset;
            else if (type == 2)
                HurtNumberText.spriteAsset = BlueNumberTextSpriteAsset;
            else
                HurtNumberText.spriteAsset = NumberTextSpriteAsset;

            HurtNumberText.text = Tools.NumToArtNum(value.ToString());
            Sequence seq = DOTween.Sequence()
            .Append(HurtNumberText.transform.DOMoveY(_MoveY, 0.4f).SetRelative().SetEase(Ease.InSine))
            .Join(HurtNumberText.transform.DOScale(new Vector3(_Scale, _Scale, _Scale), 0.4f).SetEase(Ease.InSine))
            .Append(HurtNumberText.transform.DOMoveY(_MoveY, 0.2f).SetRelative().SetEase(Ease.OutSine))
            .Join(HurtNumberText.transform.DOScale(new Vector3(_OldScale, _OldScale, _OldScale), 0.2f).SetEase(Ease.OutSine))
            .AppendInterval(0.4f)
            .Append(HurtNumberText.transform.DOMoveY(_OldMoveY, 0.5f).SetRelative().SetEase(Ease.OutSine))
            .Join(HurtNumberText.DOFade(0, 0.5f).SetEase(Ease.OutSine))
            .OnComplete(() =>
            {
                HurtNumberText.text = "";
                Destroy(HurtNumberText_tran.gameObject, 3f);
            });
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateLifeSlider(int value1 = 1, int value2 = 1)
    {
        float value = (float)value1 / (float)value2;
        LifeFill.fillAmount = value;
    }

    public void UpdateAttackSlider(float value1 = 0, float value2 = 0)
    {
        float value = value1 / value2;

        if (value1 == 0 && value2 == 0)
        {
            AttackFill.fillAmount = 0;
        }
        else
        {
            AttackFill.fillAmount = value;
        }
    }

    public void HideEnemySlider()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowAndHideAttackSlider(bool isShow)
    {
        AttackSlider.gameObject.SetActive(isShow);
    }

    public void UpdateSlider(int value1 = 1, int value2 = 1)
    {
        float value = (float)value1 / (float)value2;
        Fill.fillAmount = value;
    }
}
