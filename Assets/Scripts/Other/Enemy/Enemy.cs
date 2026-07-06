using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Walking,
    GetHit,
    Dead,
    Attack,
}

public interface Enemy
{
    public void Init();

    public void Hurt(int _damage, int _getHurtEF);

    public void PlayerGetLockEnemy();

    public void ShowSelectCircle();

    public void HideSelectCircle();

    
    public void ShowGetHurtEF(int _index);

    public void StartGame();
}


