using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayGroundManager : MonoBehaviour
{
    
    public GameObject[] Walls;

    
    public GameObject[] StartWalls;

    
    public GameObject[] SpawnEnemyWalls;

    
    public bool IsTrigger = true;

    
    public Transform DropPoint;

    
    public TerrainRotator terrainRotator;

    
    public GameObject ArrowFont;

    public Vector3[] ArrowFontAppearPos;

    void Start()
    {
        foreach (var item in Walls)
        {
            item.SetActive(false);
        }
    }

    public void HideArrowFont()
    {
        ArrowFont.SetActive(false);
    }
    public void AppearArrowFont()
    {
        ArrowFont.SetActive(true);
    }
    public void ChangeArrowFontPos(int index)
    {
        if (!GameManager.Instance.EnemyManager.isMaxLevel())
        {
            ArrowFont.SetActive(true);
            ArrowFont.transform.localPosition = ArrowFontAppearPos[index];
        }
    }

    public void ShowStartWalls()
    {
        int index = GameManager.Instance.EnemyManager.GetLevel();
        StartWalls[index].SetActive(true);
        if (SpawnEnemyWalls[index] != null)
            SpawnEnemyWalls[index].SetActive(true);
        Walls[index].SetActive(false);
        
    }

    public void StartTerrainRotator()
    {
        terrainRotator.enabled = true;
        

    }

    public void StopTerrainRotator()
    {
        terrainRotator.LoseAndStopRotation();
        terrainRotator.enabled = false;
    }

    public void WinStopTerrainRotator()
    {
        terrainRotator.WinAndStopRotation();
    }

    public void HideStartWalls()
    {
        int index = GameManager.Instance.EnemyManager.GetLevel();
        if (SpawnEnemyWalls[index] != null)
            SpawnEnemyWalls[index].SetActive(false);
        StartWalls[index].SetActive(false);
        Walls[index].SetActive(true);
        
    }

    public void ShowAndHideTrigger(bool isShow)
    {
        IsTrigger = isShow;
    }
}
