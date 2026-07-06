using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Canvas canvas;                
    public float followSpeed = 20f;      
    public float returnSpeed = 10f;      
    public float edgeThreshold = 110f;     

    RectTransform rectTransform;
    Vector2 startAnchoredPos;            
    bool isDragging = false;
    bool isReturning = false;            
    bool isHidden = false;               
    Vector2 dragTargetPos;               

    public bool isWeaponUp = false;

    public bool isEquipUp = false;

    private bool isSell = false;

    private bool isChange = false;

    private Animator animator;

    public int SellCoin = 0;

    public GameObject TipPg;
    public GameObject Fx_ScreenEdge_Green;


    public int AddPowerNumer = 0;

    private ParticleSystem Fx_ScreenEdge_Green_Particle;

    public GameObject Fx_ScreenEdge_Red;

    private ParticleSystem Fx_ScreenEdge_Red_Particle;

    public bool isSellUp = true;

    public GameObject AppearCard;

    public GameObject Finger;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    void OnEnable()
    {
        animator.CrossFade("CardShake", 0.2f);
        
        Finger.SetActive(true);
        GameManager.Instance.SetStop(true);
    }

    void OnDisable()
    {
        
        GameManager.Instance.SetStop(false);
        Finger.SetActive(false);
    }

    void Start()
    {
        startAnchoredPos = rectTransform.anchoredPosition;
        dragTargetPos = startAnchoredPos;
        Fx_ScreenEdge_Green.SetActive(false);
        
        Fx_ScreenEdge_Red.SetActive(false);
        animator.CrossFade("CardShake", 0.2f);
    }

    void Update()
    {
        if (isHidden)
            return;

        if (isDragging)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                dragTargetPos,
                followSpeed * Time.deltaTime
            );
        }

        else if (isReturning)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                startAnchoredPos,
                returnSpeed * Time.deltaTime
            );


            if (Vector2.SqrMagnitude(rectTransform.anchoredPosition - startAnchoredPos) < 0.1f * 0.1f)
            {
                rectTransform.anchoredPosition = startAnchoredPos;
                isReturning = false;

                TipPg.SetActive(true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isHidden)
            return;


        if (isReturning)
            return;

        TipPg.SetActive(false);

        animator.CrossFade("Stop", 0.2f);

        isDragging = true;


        UpdateDragTarget(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || isHidden)
            return;

        UpdateDragTarget(eventData);

        if (IsNearScreenEdge())
        {

            HideCard();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging || isHidden)
            return;

        animator.CrossFade("CardShake", 0.2f);

        isDragging = false;


        if (IsNearScreenEdge())
        {

            HideCard();
        }
        else
        {

            isReturning = true;
        }
    }


    void UpdateDragTarget(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        Vector2 localPos;

        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos))
        {
            
            dragTargetPos = new Vector2(localPos.x, startAnchoredPos.y);
        }
    }



    
    bool IsNearScreenEdge()
    {
        Vector3 worldPos = rectTransform.position;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            worldPos
        );

        float distToLeft = screenPos.x;                       
        float distToRight = Screen.width - screenPos.x;       

        
        if (isSellUp && distToLeft <= edgeThreshold)
        {
            Fx_ScreenEdge_Red.SetActive(true);
            isSell = true;
            return true;
        }
        else if (distToRight <= edgeThreshold)
        {
            Fx_ScreenEdge_Green.SetActive(true);
            isChange = true;
            return true;
        }
        Fx_ScreenEdge_Green.SetActive(false);
        Fx_ScreenEdge_Red.SetActive(false);
        isSell = false;
        isChange = false;

        return false;
    }

    
    void HideCard()
    {
        isHidden = true;
        isDragging = false;
        isReturning = false;

        
        if (isSell)
        {
            
            UIGame.Instance.AddCoin(SellCoin);
            AudioManager.Instance.PlaySound("GetCoin");
            AudioManager.Instance.PlaySound("MoveCard");
        }
        else if (isChange)
        {
            if (isWeaponUp)
            {
                GameManager.Instance.Player.ChangeCharacterWeapon(2);
                UIGame.Instance.ShowNewWeapon();
            }
            else if (isEquipUp)
            {
                GameManager.Instance.Player.ChangeCharacter(2);
                UIGame.Instance.ShowNewEquip();
            }
            UIGame.Instance.AddPower(AddPowerNumer);
            AudioManager.Instance.PlaySound("MoveCard");
        }

        
        transform.parent.gameObject.SetActive(false);
        

        
    }

    
    public void ResetCard()
    {
        isHidden = false;
        isDragging = false;
        isReturning = false;

        rectTransform.anchoredPosition = startAnchoredPos;
        gameObject.SetActive(true);
    }
}
