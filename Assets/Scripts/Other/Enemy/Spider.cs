using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Spider : MonoBehaviour, Enemy
{
    private Transform target;

    public float moveSpeed = 7f;         
    public float rotationSpeed = 10f;    
    public float stopDistance = 2.7f;    

    private bool isHaveEquipment = false;

    private bool isDoubleHaveEquipment = false;
    
    private float coolAttackTime = 1.2f;    

    private float attackTime = 1f;        

    private bool isAttack = false;

    private Animator animator;

    private bool isDead = false;

    
    private LifeLookCamera lifeLookCamera;

    public int maxHealth = 90;

    private int currentHealth = 0;

    private EnemyState state = EnemyState.Idle;

    public bool isGetHurt = false;

    private float getHitCoolTimer = 0.2f;

    private float getHitTimer = 0f;

    private bool isCanGetHurt = true;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    private Material material;

    
    public GameObject EquipBallPrefabs;

    public Transform EquipPoint;

    
    public GameObject coinPrefab;

    public bool isCanDropCoin = true;

    
    public GameObject Fx_SelectCircle_01;

    public GameObject[] Fx_GetHurtEF; 

    private float AttackDistance = 3.2f;

    private Collider EnemyColider;

    private bool isStartGame;

    
    public void StartGame()
    {
        isStartGame = true;
        SetState(EnemyState.Walking);
    }

    public void SetLife(int _life)
    {
        maxHealth = _life;
        AttackDistance = 5f;
    }

    public void SetState(EnemyState newState)
    {
        if (isDead)
            return;
        if (state != newState)
        {
            switch (newState)
            {
                case EnemyState.Walking:
                    animator.CrossFade("Walk", 0.2f);
                    break;
                case EnemyState.Dead:
                    animator.CrossFade("Dead", 0.2f);
                    break;
                case EnemyState.Attack:
                    animator.CrossFade("Atk", 0.2f);
                    break;
            }
        }
        state = newState;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        lifeLookCamera = GetComponentInChildren<LifeLookCamera>();
        material = skinnedMeshRenderer.material;
        EnemyColider = GetComponent<Collider>();
    }

    
    void Start()
    {
        Init();
    }

    public void Init()
    {
        target = GameManager.Instance.Player.transform;
        currentHealth = maxHealth;
        Fx_SelectCircle_01.SetActive(false);
        EnemyColider.enabled = false;
        Invoke("ShowColider", 0.5f);
    }

    public void ShowColider()
    {
        EnemyColider.enabled = true;
    }

    
    public void HaveEquipment(bool _isHaveEquipment)
    {
        isHaveEquipment = _isHaveEquipment;
    }

    public bool IsHaveEquipment()
    {
        return isHaveEquipment;
    }

    public void DoubleHaveEquipment(bool _isDoubleHaveEquipment)
    {
        isDoubleHaveEquipment = _isDoubleHaveEquipment;
    }



    
    void Update()
    {
        if (GameManager.Instance.Pause())
            return;
        if (isDead)
            return;
        if (!isStartGame)
            return;
        PlayerGetLockEnemy();

        DontGetManyHurt();

        FindPlayer();
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

    
    void FindPlayer()
    {
        if (target == null)
        {
            return;
        }

        if (isGetHurt)
            return;

        
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        if (distance < 0.001f)
            return;

        Vector3 dir = toTarget.normalized;

        
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        
        if (distance > stopDistance && !isAttack)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else if (!isAttack)
        {
            isAttack = true;
            StartCoroutine(Attack());
        }

        if (attackTime < coolAttackTime)
        {
            attackTime += Time.deltaTime;
        }
    }

    
    public void Hurt(int _damage, int _EFindex)
    {
        if (!isCanGetHurt)
            return;
        AudioManager.Instance.PlaySound("EnemyHurt");
        ShowGetHurtEF(_EFindex);
        StartCoroutine(GetHurt(_damage, _EFindex));
    }

    
    IEnumerator Attack()
    {
        if (attackTime >= coolAttackTime)
        {
            SetState(EnemyState.Attack);
            AudioManager.Instance.PlaySound("BossAttack");
            yield return new WaitForSeconds(1f);
            if (Vector3.Distance(transform.position, target.position) < AttackDistance && !isDead)
            {
                
                GameManager.Instance.Player.Hurt(40);
            }
            attackTime = 0f;
            yield return new WaitForSeconds(0.2f);
            SetState(EnemyState.Walking);
        }
        isAttack = false;
    }

    
    IEnumerator GetHurt(int _damage, int _EFindex)
    {
        currentHealth -= _damage;
        lifeLookCamera.UpdateSlider(currentHealth, maxHealth);
        lifeLookCamera.AppearHurtNumber(_damage, 3, _EFindex);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            SetState(EnemyState.Dead);
            isDead = true;
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Player.ClearEnemyLock(this.transform);
            HideSelectCircle();
            
            yield return new WaitForSeconds(0.3f);
            UIGame.Instance.AddExperience(Random.Range(500, 700));
            
            lifeLookCamera.HideEnemySlider();
            yield return new WaitForSeconds(1f);
            
            if (isHaveEquipment)
            {
                GameObject _EquipBall = Instantiate(EquipBallPrefabs, EquipPoint, false);
                _EquipBall.GetComponent<EquipBall>().SetLevel(0);
                _EquipBall.GetComponent<EquipBall>().AppearCurrentChild(0);
                _EquipBall.transform.parent = null;
                _EquipBall.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                GameManager.Instance.EnemyManager.DoubleSetEquip(0);
                
            }
            if (isDoubleHaveEquipment)
            {
                GameObject _EquipBall = Instantiate(EquipBallPrefabs, EquipPoint, false);
                _EquipBall.GetComponent<EquipBall>().SetLevel(1);
                _EquipBall.GetComponent<EquipBall>().AppearCurrentChild(1);
                _EquipBall.transform.parent = null;
                _EquipBall.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                GameManager.Instance.EnemyManager.DoubleSetEquip(1);
            }
            GameManager.Instance.EnemyManager.CloseEnemyCount();
            DropCoin();
            gameObject.SetActive(false);
            Destroy(gameObject, 3f);
        }
        else
        {
            isCanGetHurt = false;
            
            isGetHurt = true;
            
            yield return new WaitForSeconds(0.3f);
            
            isGetHurt = false;
        }
    }

    public void DropCoin()
    {
        if (!isCanDropCoin)
            return;
        AudioManager.Instance.PlaySound("DropCoin");
        for (int i = 0; i < 3; i++)
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

    
    public void SetNoCanDropCoin()
    {
        isCanDropCoin = false;
    }


    public void ShowGetHurtEF(int _index)
    {
        GameObject EF = Instantiate(Fx_GetHurtEF[_index], transform, false);
        EF.transform.position += new Vector3(0, 1.5f, 0);
        EF.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Destroy(EF, 3f);
    }

    public void PlayerGetLockEnemy()
    {
        float _Distance = Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position);
        if (_Distance < 10f)
            GameManager.Instance.Player.GetLockEnemy(transform, _Distance);
        else
        {
            GameManager.Instance.Player.ClearEnemyLock(transform);
        }
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
