using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    protected bool isGetEquip = false;

    protected int enemyCount = 0;

    protected virtual void Start()
    {
        enemyCount = transform.childCount;
    }

    public void AddEnemyCount()
    {
        enemyCount++;
    }

    public virtual void StopStartEvent()
    {

    }

    public void SetChildStartGame()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Enemy>() != null)
                transform.GetChild(i).GetComponent<Enemy>().StartGame();
        }
    }

    public void CloseEnemyCount()
    {
        enemyCount--;
        if (enemyCount == 0)
        {
            GameManager.Instance.PlayGroundManager.ChangeArrowFontPos(GameManager.Instance.EnemyManager.GetLevel());
            GameManager.Instance.EnemyManager.LevelUp();
            GameManager.Instance.Player.ClearEnemyLock();
            GameManager.Instance.EnemyManager.DestroyEnemy(5f);
            GameManager.Instance.PlayGroundManager.ShowAndHideTrigger(true);
            
            if (!GameManager.Instance.EnemyManager.isMaxLevel())
            {
                GameManager.Instance.EnemyManager.SpawnEnemy();
            }
        }
    }
}
