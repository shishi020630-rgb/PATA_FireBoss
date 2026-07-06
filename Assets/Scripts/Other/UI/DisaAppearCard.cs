using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisaAppearCard : MonoBehaviour, IPointerDownHandler
{
    public GameObject DisappearText;

    public int index;

    private bool isClicked = false;

    public bool isOneEndGuide = false;

    public bool isDisappear = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isClicked)
            return;
        DisappearText.SetActive(false);
        isClicked = false;
        if (!isOneEndGuide)
            UIController.Instance.GuidePg.PlayAnimatorEndStartGame(index);
        else
            UIController.Instance.GuidePg.PlayAnimatorOneEnd();
    }

    
    private void OnEnable()
    {
        if (!isDisappear) return;
        DisappearText.SetActive(false);
        if (!isOneEndGuide)
            Invoke("AppearText", 0.5f);
        else
            Invoke("AppearOneEndText", 0.5f);
        isDisappear = false;
    }

    void AppearOneEndText()
    {
        isClicked = true;
    }

    void OnDisable()
    {
        isDisappear = true;
    }

    void AppearText()
    {
        DisappearText.SetActive(true);
        isClicked = true;
    }

    
    void Update()
    {

    }
}
