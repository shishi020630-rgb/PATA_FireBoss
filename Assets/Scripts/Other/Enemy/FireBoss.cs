using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoss : MonoBehaviour, Enemy
{
    private Transform target;

    public float rotationSpeed = 10f;    
    private bool isHaveEquipment = false;
    
    private float coolAttackTime = 3f;    

    private float attackTime = 0f;        

    private bool isAttack = true;

    private Animator animator;

    
    public GameObject fireBallPrefab;

    public Transform FirePoint;

    public int fireBallDamage = 50;

    private bool isDead = false;

    
    private LifeLookCamera lifeLookCamera;

    private int maxHealth = 2400;

    private int currentHealth = 0;

    private EnemyState state = EnemyState.Idle;

    private bool isGetHurt = false;

    public bool isLockAttack = false;

    private float getHitCoolTimer = 0.2f;

    private float getHitTimer = 0f;

    private bool isCanGetHurt = true;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    private Material material;

    
    public GameObject Fx_SelectCircle_01;

    public GameObject[] Fx_GetHurtEF; 

    public void SetState(EnemyState newState)
    {
        if (isDead)
            return;
        if (state != newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    if (!isLockAttack)
                    {
                        animator.CrossFade("Idle", 0.2f);
                        state = newState;
                    }
                    break;
                case EnemyState.GetHit:
                    if (!isLockAttack)
                    {
                        animator.CrossFade("GetHit", 0.2f);
                        state = newState;
                    }
                    break;
                case EnemyState.Dead:
                    animator.CrossFade("Dead", 0.2f);
                    state = newState;
                    break;
                case EnemyState.Attack:
                    animator.CrossFade("Atk", 0.2f);
                    state = newState;
                    break;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        lifeLookCamera = GetComponentInChildren<LifeLookCamera>();
        material = skinnedMeshRenderer.material;
    }

    
    void Start()
    {
        Init();
    }

    public void Init()
    {
        GameManager.Instance.SetStop(true);
        lifeLookCamera.gameObject.SetActive(false);
        target = GameManager.Instance.Player.transform;
        currentHealth = maxHealth;
        Fx_SelectCircle_01.SetActive(false);
        Invoke("SetStartGame", 1.6f);
    }

    public void SetStartGame()
    {
        lifeLookCamera.gameObject.SetActive(true);
        
        
        GameManager.Instance.SetStop(false);

        GameManager.Instance.PlayGroundManager.StartTerrainRotator();
    }

    
    public void StartGame()
    {

    }

    
    public void HaveEquipment(bool _isHaveEquipment)
    {
        isHaveEquipment = _isHaveEquipment;
    }

    
    void Update()
    {
        if (GameManager.Instance.Pause())
            return;
        if (isDead)
            return;

        PlayerGetLockEnemy();

        

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

        

        if (!isAttack)
        {
            StartCoroutine(Attack());
        }
        else
        {
            
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            Vector3 dir = toTarget.normalized;

            
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (attackTime < coolAttackTime)
        {
            attackTime += Time.deltaTime;
            if (attackTime >= coolAttackTime)
            {
                isAttack = false;
            }
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
            isLockAttack = true;
            SetState(EnemyState.Attack);
            isAttack = true;
            yield return new WaitForSeconds(0.5f);
            GameObject fireBall = Instantiate(fireBallPrefab, FirePoint, false);
            fireBall.transform.parent = null;
            fireBall.GetComponent<FireBall>().SetDamage(fireBallDamage);
            yield return new WaitForSeconds(0.5f);

            attackTime = 0f;
            yield return new WaitForSeconds(0.2f);
            isLockAttack = false;
            SetState(EnemyState.Idle);
        }
    }

    
    IEnumerator GetHurt(int _damage, int _EFindex)
    {
        currentHealth -= _damage;
        lifeLookCamera.UpdateSlider(currentHealth, maxHealth);
        lifeLookCamera.AppearHurtNumber(_damage, 5, _EFindex);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            SetState(EnemyState.Dead);
            isDead = true;
            GameManager.Instance.PlayGroundManager.terrainRotator.SetBossIsDead();
            UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
            AudioManager.Instance.PlaySound("BossDead");
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Player.ClearEnemyLock(this.transform);
            HideSelectCircle();
            GameManager.Instance.SetStop(true);
            
            yield return new WaitForSeconds(0.3f);
            UIGame.Instance.AddExperience(Random.Range(1500, 2000));
            
            yield return new WaitForSeconds(0.8f);
            lifeLookCamera.HideEnemySlider();
            GameManager.Instance.SetStop(false);
            GameManager.Instance.SetWin();
            if (isHaveEquipment)
            {
                GameManager.Instance.EnemyManager.SetEquip();
            }
            
            GameManager.Instance.EnemyManager.CloseEnemyCount();
            gameObject.SetActive(false);
            Destroy(gameObject, 3f);
        }
        else
        {
            
            SetState(EnemyState.GetHit);
            isGetHurt = true;
            if (!isLockAttack)
            {
                
                yield return new WaitForSeconds(0.3f);
                
            }
            isGetHurt = false;
            SetState(EnemyState.Idle);
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
        if (_Distance < 100f)
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
