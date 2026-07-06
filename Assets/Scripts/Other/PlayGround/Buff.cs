using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum BuffType
{
    None,

    DoubleArrow,

    AddSpeed,
    Fire,
    Water,
}

public class Buff : MonoBehaviour
{
    public BuffType type = BuffType.None;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.Player.GetBuff(type);
            if (type == BuffType.AddSpeed)
            {
                GameManager.Instance.Player.ChangeStepsMusic(1);
            }
            else
            {
                GameManager.Instance.Player.ChangeStepsMusic(2);
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.Player.GetBuff(BuffType.None);
            GameManager.Instance.Player.ChangeStepsMusic(1);
        }
    }
}
