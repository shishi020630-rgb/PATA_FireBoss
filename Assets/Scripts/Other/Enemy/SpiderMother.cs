using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiderMother : MonoBehaviour, Enemy
{
    public Transform SpiderMotherAnimator;

    public Transform SpiderSpwanPoint;

    public GameObject SpiderPrefab;

    private float SpawnTimer = 0f;
    public float SpawnInterval = 5f;
    private bool isDead = false;
    private LifeLookCamera lifeLookCamera;

    private Animator animator;

    private EnemyState state = EnemyState.Idle;

    private int maxHealth = 780;

    private int currentHealth = 0;

    private bool isCanSpawn = true;

    public bool isHaveEquipment = false;

    private float getHitCoolTimer = 0.2f;

    private float getHitTimer = 0f;

    private bool isCanGetHurt = true;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    private Material material;

    
    public GameObject EquipBallPrefabs;

    public Transform EquipPoint;

    
    public GameObject coinPrefab;

    
    public GameObject Fx_SelectCircle_01;

    public GameObject[] Fx_GetHurtEF; 

    private bool isStartGame;

    public void StartGame()
    {
        Invoke("SetStartGame", 1.2f);
    }

    private void SetStartGame()
    {
        isStartGame = true;
        
        
        GameManager.Instance.SetStop(false);
                                            

        lifeLookCamera.gameObject.SetActive(true);
    }


    public void SetState(EnemyState newState)
    {
        if (isDead)
            return;
        if (state != newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    animator.CrossFade("Idle", 0.2f);
                    break;
                case EnemyState.Attack:
                    animator.CrossFade("Atk", 0.2f);
                    break;
            }
        }
        state = newState;
    }

    void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        lifeLookCamera = GetComponentInChildren<LifeLookCamera>();
        animator = GetComponent<Animator>();
        material = skinnedMeshRenderer.material;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        currentHealth = maxHealth;
        Fx_SelectCircle_01.SetActive(false);
        lifeLookCamera.gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (GameManager.Instance.Pause())
            return;
        if (isDead)
            return;
        if (!isStartGame)
            return;
        if (SpawnTimer < SpawnInterval && isCanSpawn)
        {
            SpawnTimer += Time.deltaTime;
            if (SpawnTimer >= SpawnInterval)
            {
                StartCoroutine(SpawnSpider());
                SpawnTimer = 0f;
            }
        }

        DontGetManyHurt();

        PlayerGetLockEnemy();
    }

    public void DontGetManyHurt()
    {
        if (!isCanGetHurt)
        {
            if (getHitTimer < getHitCoolTimer)
            {
                getHitTimer += Time.deltaTime;
                if (getHitTimer >= getHitCoolTimer)
                {
                    isCanGetHurt = true;
                }
            }
        }
        else
        {
            getHitTimer = 0f;
        }
    }

    
    public void HaveEquipment(bool _isHaveEquipment)
    {
        isHaveEquipment = _isHaveEquipment;
    }

    IEnumerator SpawnSpider()
    {
        isCanSpawn = false;
        
        SetState(EnemyState.Attack);
        yield return new WaitForSeconds(0.1f);
        int RandomIndex = Random.Range(1, 4);
        for (int i = 0; i < RandomIndex; i++)
        {
            SpiderMotherAnimator.localScale = new Vector3(1f, 1f, 1f);
            Transform _SpiderPrefab = Instantiate(SpiderPrefab, SpiderSpwanPoint, false).transform;
            _SpiderPrefab.SetParent(transform.parent);
            _SpiderPrefab.GetComponent<Spider>().SetNoCanDropCoin();
            
            _SpiderPrefab.GetComponent<Spider>().StartGame();
            _SpiderPrefab.localScale = new Vector3(0.5833794f, 0.5833794f, 0.5833794f);
            GameManager.Instance.EnemyManager.AddEnemyCount();
            _SpiderPrefab.GetComponent<Spider>().SetLife(120);
            yield return new WaitForSeconds(0.2f);
        }
        isCanSpawn = true;
        SetState(EnemyState.Idle);
    }

    
    public void Hurt(int _damage, int _EFindex)
    {
        if (!isCanGetHurt)
            return;
        AudioManager.Instance.PlaySound("EnemyHurt");
        ShowGetHurtEF(_EFindex);
        StartCoroutine(GetHurt(_damage, _EFindex));
    }

    
    IEnumerator GetHurt(int _damage, int _EFindex)
    {
        if (isCanGetHurt)
        {
            currentHealth -= _damage;
            lifeLookCamera.UpdateSlider(currentHealth, maxHealth);
            lifeLookCamera.AppearHurtNumber(_damage, 3, _EFindex);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                GetComponent<Collider>().enabled = false;
                GameManager.Instance.Player.ClearEnemyLock(this.transform);
                HideSelectCircle();
                
                yield return new WaitForSeconds(0.3f);
                UIGame.Instance.AddExperience(Random.Range(800, 1100));
                
                lifeLookCamera.HideEnemySlider();
                yield return new WaitForSeconds(0.5f);
                
                if (isHaveEquipment)
                {
                    GameObject _EquipBall = Instantiate(EquipBallPrefabs, EquipPoint, false);
                    _EquipBall.GetComponent<EquipBall>().SetLevel(1);
                    _EquipBall.GetComponent<EquipBall>().AppearCurrentChild(1);
                    GameManager.Instance.EnemyManager.SetEquip();
                    _EquipBall.transform.parent = null;
                    _EquipBall.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                }
                DropCoin();
                GameManager.Instance.EnemyManager.CloseEnemyCount();
                gameObject.SetActive(false);
                Destroy(gameObject, 3f);
            }
            else
            {
                isCanGetHurt = false;
                
            }
        }

    }


    public void DropCoin()
    {
        AudioManager.Instance.PlaySound("DropCoin");
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-0.9f, 0.9f),
                Random.Range(0f, 0.5f),
                Random.Range(-0.9f, 0.9f)
            );

            spawnPos.y += 0.25f; 

            GameObject c = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            var fly = c.GetComponent<DropCoin>();
            fly.SetTargetUI(UIGame.Instance.GetCoinIconRect());
            fly.flyDuration = Random.Range(0.4f, 0.8f); 
            c.transform.parent = GameManager.Instance.DropPoints();
        }
        
    }

    


    public void ShowGetHurtEF(int _index)
    {
        GameObject EF = Instantiate(Fx_GetHurtEF[_index], transform, false);
        EF.transform.position += new Vector3(0, 1.5f, 0);
        EF.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        Destroy(EF, 3f);
    }

    public void PlayerGetLockEnemy()
    {
        float _Distance = Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position);
        if (_Distance < 15f)
            GameManager.Instance.Player.GetLockEnemy(transform, _Distance);
        else
            GameManager.Instance.Player.ClearEnemyLock(transform);
    }

    public void ShowSelectCircle()
    {
        Fx_SelectCircle_01.SetActive(true);
    }

    public void HideSelectCircle()
    {
        Fx_SelectCircle_01.SetActive(false);
    }
}
