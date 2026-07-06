using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJoystick : MonoBehaviour
{
    
    public RectTransform background;   
    public RectTransform handle;       

    
    public float handleLimit = 100f;   

    public Vector2 Direction { get; private set; }

    Camera _uiCamera;
    Canvas _canvas;

    void Awake()
    {
        if (background == null)
            background = GetComponent<RectTransform>();

        _canvas = GetComponentInParent<Canvas>();

        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            _uiCamera = _canvas.worldCamera;

        gameObject.SetActive(false);
    }

    
    public void ShowAtPosition(Vector2 screenPos, bool isShow = true)
    {
        gameObject.SetActive(isShow);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            _uiCamera,
            out localPos);

        
        background.anchoredPosition = localPos;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;

        Direction = Vector2.zero;
    }

    
    public void UpdateDrag(Vector2 screenPos)
    {
        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                screenPos,
                _uiCamera,
                out localPos))
        {
            Vector2 clampedPos = localPos;

            if (clampedPos.magnitude > handleLimit)
                clampedPos = clampedPos.normalized * handleLimit;

            if (handle != null)
                handle.anchoredPosition = clampedPos;

            Direction = clampedPos / handleLimit;
        }
    }

    public void Hide()
    {
        Direction = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;

        gameObject.SetActive(false);
    }
}
