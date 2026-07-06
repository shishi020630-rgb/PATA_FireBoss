using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleMonoBase<GameManager>
{
    public Player Player;

    public EnemyManager EnemyManager;

    public PlayGroundManager PlayGroundManager;

    public Transform[] PlayerRespawnPoints;

    public GameObject WinEndAnim;

    public System.Action OnRestart;

    private bool isWin = false;

    private bool isLose = false;

    private bool isStop = false;

    private bool isPause = false;
    
    [Range(0, 1)]
    public float BkMusicVolume = 0.05f;

    private float lastBkMusicVolume = 0;

    void Start()
    {
        AudioManager.Instance.PlayBackMusic("BGM");
        AudioManager.Instance.ChangeSoundValue(0.3f);
    }

    void Update()
    {
        if (lastBkMusicVolume != BkMusicVolume)
        {
            lastBkMusicVolume = BkMusicVolume;
            AudioManager.Instance.SetSoundVaule(BkMusicVolume);
        }
    }

    void OnEnable()
    {
        OnRestart += PlayerRestart;
        OnRestart += EnemyRestart;
        OnRestart += UIRestart;
        OnRestart += DelayWallRestart;
        OnRestart += ClearAllDropObjects;
    }

    void OnDisable()
    {
        OnRestart -= PlayerRestart;
        OnRestart -= EnemyRestart;
        OnRestart -= UIRestart;
        OnRestart -= DelayWallRestart;
        OnRestart -= ClearAllDropObjects;
    }

    void OnDestroy()
    {
        OnRestart -= PlayerRestart;
        OnRestart -= EnemyRestart;
        OnRestart -= UIRestart;
        OnRestart -= DelayWallRestart;
        OnRestart -= ClearAllDropObjects;
    }

    public void IsPause()
    {
        if (isWin || isLose || isStop)
        {
            isPause = true;
            UIGame.Instance.RestartClearjoystickFingerId();
        }
        else
        {
            isPause = false;
        }
        UIGame.Instance.StopTouchArea();
        Player.GetMoveDir(Vector3.zero);
        UIGame.Instance.ShowTouchArea(!isPause);
    }

    public bool Pause()
    {
        return isPause;
    }

    
    public void SetStop(bool _stop)
    {
        isStop = _stop;
        if (isStop)
            Player.SetState(PlayerState.Idle);
        IsPause();
    }

    public void SetWin()
    {
        isWin = true;
        
        Player.SetState(PlayerState.Idle);
        UIGame.Instance.StopTouchArea();
        Player.GetMoveDir(Vector3.zero);
        UIGame.Instance.ShowTouchArea(false);
        AudioManager.Instance.PauseBackMusic();
        UIGame.Instance.RestartClearjoystickFingerId();
        Instantiate(WinEndAnim, transform, false);
        PlayGroundManager.WinStopTerrainRotator();
        Invoke("PlayGetEquipSound", 0.3f);
        Invoke("ShowEndPlay", 1.6f);
        IsPause();
    }

    private void PlayGetEquipSound()
    {
        AudioManager.Instance.PlaySound("GetEquip");
    }

    private void ShowEndPlay()
    {
        UIController.Instance.ShowEndPlayGround(true);
        AudioManager.Instance.PauseBackMusic();
        AudioManager.Instance.PlaySound("Victory");
    }

    
    public void SetLose(bool _lose)
    {
        isLose = _lose;
        UIController.Instance.ShowEndPlayGround(false);
        AudioManager.Instance.PauseBackMusic();
        AudioManager.Instance.PlaySound("Defeat");
        IsPause();
    }

    public bool Win()
    {
        return isWin;
    }

    public bool Lose()
    {
        return isLose;
    }

    public bool Stop()
    {
        return isStop;
    }

    public void Restart()
    {
        isWin = false;
        isLose = false;
        isStop = false;
        IsPause();

        OnRestart?.Invoke();

    }

    private void PlayerRestart()
    {
        
        Player.Respawn(PlayerRespawnPoints[EnemyManager.GetLevel()].position);
        AudioManager.Instance.PlayBackMusic("BGM");
        
    }

    public Transform DropPoints()
    {
        return PlayGroundManager.DropPoint;
    }

    private void ClearAllDropObjects()
    {
        
        for (int i = 0; i < PlayGroundManager.DropPoint.childCount; i++)
        {
            Destroy(PlayGroundManager.DropPoint.GetChild(i).gameObject);
        }
        GC.Collect();
    }

    private void EnemyRestart()
    {
        
        EnemyManager.DestroyEnemy(0f);
        if (!EnemyManager.isMaxLevel())
        {
            EnemyManager.SpawnEnemy();
        }
    }

    private void UIRestart()
    {
        
        UIController.Instance.Restart();
        UIGame.Instance.RestartClearjoystickFingerId();
    }

    private void DelayWallRestart()
    {
        Invoke("WallRestart", 0.2f);
    }

    private void WallRestart()
    {
        PlayGroundManager.ShowStartWalls();
        PlayGroundManager.StopTerrainRotator();
        PlayGroundManager.AppearArrowFont();
    }
}
