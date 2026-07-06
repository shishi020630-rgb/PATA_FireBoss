using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    public int MaxLevel = 2;

    
    public int Level = -1;
    
    public GameObject[] Enemys;
    
    public bool[] isHaveEquip;

    private GameObject Enemy;

    void Start()
    {
        SpawnEnemy();
    }

    
    public bool HaveEquip(int index)
    {
        return isHaveEquip[index];
    }

    
    public void SetEquip()
    {
        isHaveEquip[Level] = true;
    }

    
    public void DoubleSetEquip(int index)
    {
        isHaveEquip[index] = true;
    }

    
    public void LevelUp()
    {
        if (Level < Enemys.Length - 1)
        {
            
            Level++;
        }
    }

    public bool isMaxLevel()
    {
        return Level == MaxLevel;
    }

    
    public int GetLevel()
    {
        return Level;
    }

    
    public void SpawnEnemy()
    {
        Enemy = Instantiate(Enemys[Level], transform, false);
    }

    public void AddEnemyCount()
    {
        Enemy.GetComponent<LevelSetting>().AddEnemyCount();
    }

    public void CloseEnemyCount()
    {
        Enemy.GetComponent<LevelSetting>().CloseEnemyCount();
    }

    public void StopEnmeyStartEvent()
    {
        Enemy.GetComponent<LevelSetting>().StopStartEvent();
    }


    
    public void DestroyEnemy(float _delay)
    {
        Destroy(Enemy, _delay);
        GC.Collect();
    }

    public void NowLevelEnemyStart()
    {
        Enemy.GetComponent<LevelSetting>().SetChildStartGame();
    }

    
}
