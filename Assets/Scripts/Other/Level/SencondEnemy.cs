using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SencondEnemy : LevelSetting
{
    public GameObject SpiderMother;
    protected override void Start()
    {
        base.Start();
        isGetEquip = GameManager.Instance.EnemyManager.HaveEquip(1);
        if (!isGetEquip)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<SpiderMother>() != null)
                {
                    transform.GetChild(i).GetComponent<SpiderMother>().HaveEquipment(true);
                    break;
                }
            }
        }
    }

    public override void StopStartEvent()
    {
        base.StopStartEvent();
        SpiderMother.SetActive(true);
        
    }
}
