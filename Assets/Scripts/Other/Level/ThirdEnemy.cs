using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdEnemy : LevelSetting
{
    protected override void Start()
    {
        base.Start();
        isGetEquip = GameManager.Instance.EnemyManager.HaveEquip(2);
        if (!isGetEquip)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<FireBoss>() != null)
                {
                    transform.GetChild(i).GetComponent<FireBoss>().HaveEquipment(true);
                    break;
                }
            }
        }
    }
}
