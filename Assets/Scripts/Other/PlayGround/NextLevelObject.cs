using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelObject : MonoBehaviour
{
    public bool isShowGuide = true;
    public bool isSpawnEnemy = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            GameManager.Instance.PlayGroundManager.HideStartWalls();
            GameManager.Instance.PlayGroundManager.ShowAndHideTrigger(false);
            GameManager.Instance.PlayGroundManager.HideArrowFont();

            if (isShowGuide)
            {
                
            }
            else
            {
                GameManager.Instance.SetStop(true);
                if (isSpawnEnemy)
                {
                    
                    GameManager.Instance.EnemyManager.SpawnEnemy();
                }
                else
                {
                    
                    GameManager.Instance.EnemyManager.StopEnmeyStartEvent();
                }
            }

            GameManager.Instance.EnemyManager.NowLevelEnemyStart();
        }
    }
}
