using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCoin : MonoBehaviour
{
    public RectTransform targetUI;

    public float flyDuration = 0.6f;

    public AnimationCurve flyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Camera _uiCamera;       
    private Camera _worldCamera;    
    private Vector3 _startPos;      
    private float _timer;

    public bool isFly = false;
    private bool _initialized = false;

    void Awake()
    {
        
        _worldCamera = Camera.main;
    }

    void Start()
    {
        InitializeUIcameraIfPossible();
        
    }

    void InitializeUIcameraIfPossible()
    {
        if (targetUI == null)
        {
            return;
        }

        Canvas canvas = targetUI.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            
            _uiCamera = null;
            _initialized = true;
            return;
        }

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _uiCamera = null; 
        }
        else
        {
            
            _uiCamera = canvas.worldCamera;
            if (_uiCamera == null)
            {
                
                _uiCamera = Camera.main;
                Debug.LogWarning("");
            }
        }

        if (_worldCamera == null)
        {
            _worldCamera = Camera.main;
            if (_worldCamera == null)
                Debug.LogWarning("");
        }

        _initialized = true;
    }

    void Update()
    {
        
        if (!_initialized && targetUI != null)
            InitializeUIcameraIfPossible();

        if (!isFly)
        {
            
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < 2.5f)
                {
                    if (AudioManager.Instance != null) AudioManager.Instance.PlaySound("GetCoin");
                    Fly();
                }
            }
            return;
        }

        if (targetUI == null || _worldCamera == null)
            return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / flyDuration);
        if (flyCurve != null) t = flyCurve.Evaluate(t);

        
        Vector3 uiScreenPos = RectTransformUtility.WorldToScreenPoint(_uiCamera, targetUI.position);

        
        float startScreenZ = _worldCamera.WorldToScreenPoint(_startPos).z;
        uiScreenPos.z = startScreenZ;

        
        Vector3 worldTargetPos = _worldCamera.ScreenToWorldPoint(uiScreenPos);

        
        Vector3 pos = Vector3.Lerp(_startPos, worldTargetPos, t);
        
        float arcHeight = 0.5f; 
        pos += Vector3.up * Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = pos;

        
        if (_timer >= flyDuration)
        {
            
            if (UIGame.Instance != null) UIGame.Instance.AddCoin(5);
            Destroy(gameObject);
        }
    }

    public void SetTargetUI(RectTransform target)
    {
        targetUI = target;
        InitializeUIcameraIfPossible();
    }

    private void Fly()
    {
        
        _startPos = transform.position;
        _timer = 0f;
        isFly = true;
    }
}
