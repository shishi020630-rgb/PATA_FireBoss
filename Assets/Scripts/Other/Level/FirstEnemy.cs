using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEnemy : LevelSetting
{
    
    protected override void Start()
    {
        base.Start();
        isGetEquip = GameManager.Instance.EnemyManager.HaveEquip(0);
        bool isDoubleGetEquip = GameManager.Instance.EnemyManager.HaveEquip(1);
        int childcount = transform.childCount;
        int getEquip = 0;
        if (!isGetEquip)
        {
            int i = Random.Range(0, childcount);
            getEquip = i;
            transform.GetChild(i).GetComponent<Spider>().HaveEquipment(true);
            
        }
        if (!isDoubleGetEquip)
        {
            int k = Random.Range(0, childcount);
            do
            {
                if (k != getEquip)
                {
                    
                    transform.GetChild(k).GetComponent<Spider>().DoubleHaveEquipment(true);
                    break;
                }
                else
                {
                    k = Random.Range(0, childcount);
                }
            }
            while (true);
        }

    }
}
