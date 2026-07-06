using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public NewJoystick NewJoystick;

    public GameObject StartGuide;

    private Vector3 fingerPos;

    public GameObject TipGound;

    private int joystickFingerId = -1;

    private bool isStartGame = false;

    void Start()
    {
        NewJoystick.gameObject.SetActive(false);
        Invoke("IsStartGame", 3f);
    }

    private void IsStartGame()
    {
        if (!isStartGame)
        {
            StartGuide.SetActive(false);
            isStartGame = true;
            Invoke("DisappearTipGound", 3f);
        }
    }

    public void DisappearTipGound()
    {
        TipGound.SetActive(false);
    }

    public void ClearjoystickFingerId()
    {
        joystickFingerId = -1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isStartGame)
        {
            fingerPos = eventData.position;
            
            joystickFingerId = eventData.pointerId;
            NewJoystick.ShowAtPosition(eventData.position, true);
            NewJoystick.gameObject.SetActive(false);
            
            isStartGame = true;
            StartGuide.SetActive(false);
            Invoke("DisappearTipGound", 3f);
            return;
        }

        if (GameManager.Instance.Pause())
        {
            NewJoystick.Hide();
            return;
        }

        
        if (joystickFingerId != -1)
            return;

        
        joystickFingerId = eventData.pointerId;

        
        NewJoystick.ShowAtPosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        

        if (GameManager.Instance.Pause())
        {
            GameManager.Instance.Player.GetMoveDir(Vector3.zero);
            return;
        }

        
        if (eventData.pointerId != joystickFingerId)
            return;

        
        NewJoystick.UpdateDrag(eventData.position);

        
        Vector2 dir2D = NewJoystick.Direction;

        
        Vector3 moveDir = new Vector3(dir2D.x, 0f, dir2D.y);

        NewJoystick.gameObject.SetActive(true);

        GameManager.Instance.Player.GetMoveDir(moveDir.normalized);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isStartGame)
        {
            fingerPos = Vector3.zero;
            
            joystickFingerId = -1;
            return;
        }


        if (GameManager.Instance.Pause())
        {
            NewJoystick.Hide();
            GameManager.Instance.Player.GetMoveDir(Vector3.zero);
            return;
        }

        
        if (eventData.pointerId != joystickFingerId)
            return;

        NewJoystick.Hide();
        GameManager.Instance.Player.GetMoveDir(Vector3.zero);

        
        joystickFingerId = -1;
    }
}
