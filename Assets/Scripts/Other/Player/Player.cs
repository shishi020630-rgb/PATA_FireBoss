using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walking,
    GetHit,
    Dead,
    Attack,
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    
    public float moveSpeed = 11f;        
    public float rotationSpeed = 10f;   
    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 moveDir = Vector3.zero;

    
    public float gravity = -9.81f;      
    public float fallSpeed = 0f;        

    
    public Transform Bow_Weapon;

    
    public GameObject[] EquipGameObjects;   

    private Vector3 equipHandVec;

    private Quaternion equipHandQut;

    
    public GameObject[] EquipGameObjects2;  

    private Vector3 equipHandVec2;

    private Quaternion equipHandQut2;

    public Transform equipHand, newEquipHand;

    

    public GameObject WeaponGameObjects;   

    public GameObject WeaponGameObjects2; 

    private PlayerState state = PlayerState.Idle; 

    public Animator equipAnimator;
    public Animator newEquipAnimator;
    private Animator animator;

    public int maxHealth = 300;
    public int currentHealth = 0;
    private LifeLookCamera lifeLookCamera;

    private float getLifeCoolTime = 5f;

    private float getLifeTime = 0f;

    private int equipIndex = 1;

    private int weaponIndex = 1;

    private bool isDead = false;

    public bool isHurted = false;

    public Transform ArrowSpawnPoint;

    public GameObject Arrow;

    private bool isLock = false;

    private Transform LockEnemy;

    private float minDistance = 0f;

    private bool isFirstLockEnemy = false;


    public int ArrowDamage = 60;

    private float arrowCoolTime = 0.5f;

    private float arrowSpeed = 1.5f;

    private float arrowTime = 0f;

    public bool DoubleArrow = false;

    private bool isShoot = false;
    public BuffType buff = BuffType.None;

    private Vector3 shootPos, shootPos2;

    private Quaternion shootQut, shootQut2;

    private Enemy isLockEnemy = null;

    public GameObject AttackCircle;

    public GameObject EquiplUpEF_Prefab;

    public GameObject NewEquiplEF;

    public AudioClip[] Stepsounds;

    private AudioSource audiosource;

    private bool isPlay = false;

    public int AddExperience = 280;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        lifeLookCamera = GetComponentInChildren<LifeLookCamera>();
        audiosource = GetComponent<AudioSource>();
        animator = equipAnimator;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        
        equipHandVec = new Vector3(0, 0, 0);
        equipHandQut = Quaternion.Euler(0, 0, 0);
        Bow_Weapon.localPosition = equipHandVec;
        Bow_Weapon.localRotation = equipHandQut;
        shootPos = new Vector3(0.0235f, 0.0053f, -0.142f);
        shootQut = Quaternion.Euler(90, 90, 0);
        
        equipHandVec2 = new Vector3(0, 0, 0);
        equipHandQut2 = Quaternion.Euler(0, 90, 0);
        shootPos2 = new Vector3(0.0235f, 0.0053f, -0.142f);
        shootQut2 = Quaternion.Euler(180, 0, 90);

        currentHealth = maxHealth;
        lifeLookCamera.SetLife(currentHealth);
        lifeLookCamera.ShowAndHideAttackSlider(false);

        AttackCircle.SetActive(false);
        NewEquiplEF.SetActive(false);

        audiosource.clip = Stepsounds[0];

        UIGame.Instance.AddExperience(AddExperience);
    }

    void Update()
    {
        if (GameManager.Instance.Pause())
            return;

        if (isDead)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            UIGame.Instance.AddPower(280);

        ShootArrow();

        HandleMovement();
    }

    
    private void ContinueAddHeart()
    {
        if (getLifeTime < getLifeCoolTime)
        {
            getLifeTime += Time.deltaTime;
            if (getLifeTime > maxHealth)
            {
                getLifeTime = 0;
            }
        }
    }

    public void SetState(PlayerState newState)
    {
        if (state != newState)
        {
            switch (newState)
            {
                case PlayerState.Idle:
                    animator.CrossFade("Idle", 0.2f);
                    break;
                case PlayerState.Walking:
                    animator.CrossFade("Walk", 0.2f);
                    break;
                case PlayerState.Attack:
                    animator.CrossFade("Attack", 0.2f);
                    break;
                case PlayerState.GetHit:
                    animator.CrossFade("GetHit", 0.2f);
                    break;
                case PlayerState.Dead:
                    animator.CrossFade("Dead", 0.2f);
                    break;

            }
        }
        state = newState;
    }

    
    public void GetMoveDir(Vector3 _moveDir)
    {
        moveDir = _moveDir;
        if (_moveDir == Vector3.zero)
            AudioPlay(false);
    }


    public void GetLockEnemy(Transform _enemy, float _Distance)
    {
        if (!isFirstLockEnemy)
        {
            isFirstLockEnemy = true;
            minDistance = _Distance;
            LockEnemy = _enemy;
            isLockEnemy = LockEnemy.GetComponent<Enemy>();
            isLockEnemy.ShowSelectCircle();
        }
        else
        {
            if (_Distance < minDistance)
            {
                minDistance = _Distance;
                if (LockEnemy != _enemy)
                {
                    isLockEnemy = LockEnemy.GetComponent<Enemy>();
                    isLockEnemy.HideSelectCircle();
                }
                LockEnemy = _enemy;
                isLockEnemy = LockEnemy.GetComponent<Enemy>();
                isLockEnemy.ShowSelectCircle();
            }
            if (LockEnemy == _enemy)
            {
                minDistance = _Distance;
            }
        }
    }

    
    public void ShootArrow()
    {
        
        if (isHurted)
        {
            
            return;
        }


        
        if (LockEnemy == null)
        {
            isLock = false;
        }
        else
        {
            isLock = true;
        }

        if (isLock)
        {
            ShootArrowTimer();
            lifeLookCamera.ShowAndHideAttackSlider(true);
            Bow_Weapon.GetChild(0).gameObject.SetActive(true);
            if (weaponIndex == 1)
            {
                Bow_Weapon.localPosition = shootPos;
                Bow_Weapon.localRotation = shootQut;
            }
            else if (weaponIndex == 2)
            {
                Bow_Weapon.localPosition = shootPos2;
                Bow_Weapon.localRotation = shootQut2;
            }
        }
        else
        {
            lifeLookCamera.UpdateAttackSlider(0, 0);
            lifeLookCamera.ShowAndHideAttackSlider(false);
            AttackCircle.SetActive(false);
            arrowTime = 0;
            
            ResetBowWeaponPosAndQut();
        }
    }

    
    public void ClearEnemyLock(Transform _enemy = null)
    {
        if (_enemy == null)
        {
            if (LockEnemy != null)
            {
                isLockEnemy = LockEnemy.GetComponent<Enemy>();
                isLockEnemy.HideSelectCircle();
            }
            LockEnemy = null;
            isFirstLockEnemy = false;
            isLock = false;
            minDistance = 0f;
        }
        else if (LockEnemy == _enemy)
        {
            isLockEnemy = LockEnemy.GetComponent<Enemy>();
            isLockEnemy.HideSelectCircle();
            LockEnemy = null;
            isLock = false;
            isFirstLockEnemy = false;
            minDistance = 0f;
        }
    }

    
    void HandleMovement()
    {

        
        if (isLock)
        {
            Vector3 toEnemy = LockEnemy.position - transform.position;
            toEnemy.y = 0f;

            if (toEnemy.sqrMagnitude > 0.0001f)
            {
                Quaternion lookEnemyRot = Quaternion.LookRotation(toEnemy.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    lookEnemyRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        
        if (moveDir == Vector3.zero)
        {
            if (!isHurted)
            {
                if (!isLock)
                    SetState(PlayerState.Idle);
            }
            
            return;
        }

        AudioPlay(true);

        
        Vector3 inputDir = new Vector3(moveDir.x, 0f, moveDir.z);

        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        moveDirection = inputDir;

        
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            if (!isLock)
            {
                
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
                if (!isHurted)
                    SetState(PlayerState.Walking);
                
            }
        }

        
        if (controller.isGrounded)
        {
            fallSpeed = -1f; 
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
        }

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = fallSpeed;

        
        controller.Move(velocity * Time.deltaTime);
    }
    
    public void ChangeCharacter(int index)
    {
        equipIndex = index;
        if (index == 1)
        {
            foreach (var equip in EquipGameObjects)
            {
                equip.gameObject.SetActive(true);
            }

            WeaponGameObjects.transform.parent = equipHand;
            WeaponGameObjects.transform.localPosition = equipHandVec;
            WeaponGameObjects.transform.localRotation = equipHandQut;
            WeaponGameObjects2.transform.parent = equipHand;
            WeaponGameObjects2.transform.localPosition = equipHandVec2;
            WeaponGameObjects2.transform.localRotation = equipHandQut2;
            animator = equipAnimator;

            foreach (var equip2 in EquipGameObjects2)
            {
                equip2.gameObject.SetActive(false);
            }
        }
        else if (index == 2)
        {
            foreach (var equip2 in EquipGameObjects2)
            {
                equip2.gameObject.SetActive(true);
            }

            WeaponGameObjects.transform.parent = newEquipHand;
            WeaponGameObjects.transform.localPosition = equipHandVec;
            WeaponGameObjects.transform.localRotation = equipHandQut;
            WeaponGameObjects2.transform.parent = newEquipHand;
            WeaponGameObjects2.transform.localPosition = equipHandVec2;
            WeaponGameObjects2.transform.localRotation = equipHandQut2;
            GameObject EquiplUpEF = Instantiate(EquiplUpEF_Prefab, transform, false);
            Destroy(EquiplUpEF, 3f);

            ShowNewEquiplEF();

            animator = newEquipAnimator;

            foreach (var equip in EquipGameObjects)
            {
                equip.gameObject.SetActive(false);
            }
            MaxAddHealth(300);
        }
    }

    
    private void ShowNewEquiplEF()
    {
        NewEquiplEF.SetActive(true);
    }

    
    public void ChangeCharacterWeapon(int index)
    {
        weaponIndex = index;
        if (index == 1)
        {
            WeaponGameObjects.SetActive(true);
            WeaponGameObjects2.SetActive(false);
            Bow_Weapon = WeaponGameObjects.transform;

            ArrowDamage = 60;
        }
        else if (index == 2)
        {
            WeaponGameObjects.SetActive(false);
            WeaponGameObjects2.SetActive(true);
            Bow_Weapon = WeaponGameObjects2.transform;

            GameObject EquiplUpEF = Instantiate(EquiplUpEF_Prefab, transform, false);
            Destroy(EquiplUpEF, 3f);

            ArrowDamage += 40;

        }
    }

    public void ResetBowWeaponPosAndQut()
    {
        Bow_Weapon.GetChild(0).gameObject.SetActive(false);
        if (weaponIndex == 1)
        {
            Bow_Weapon.localPosition = equipHandVec;
            Bow_Weapon.localRotation = equipHandQut;
        }
        else if (weaponIndex == 2)
        {
            Bow_Weapon.localPosition = equipHandVec2;
            Bow_Weapon.localRotation = equipHandQut2;
        }
    }

    
    public void Hurt(int _damage)
    {
        if (isDead)
            return;
        if (isHurted)
            return;
        int damage = _damage;
        AudioManager.Instance.PlaySound("PlayerHurt");
        switch (equipIndex)
        {
            case 1:
                currentHealth -= damage;
                break;
            case 2:
                damage = (int)(damage * 0.8f);
                currentHealth -= damage;
                break;
        }
        lifeLookCamera.AppearHurtNumber(damage, 0, 0);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StopAllCoroutines();
            StartCoroutine(DeadAnimator());
        }
        else
        {
            
            StartCoroutine(HurtAnimator());
        }
        lifeLookCamera.UpdateLifeSlider(currentHealth, maxHealth);
        lifeLookCamera.SetLife(currentHealth);
    }

    
    public void AddHealth(int _add)
    {
        currentHealth += _add;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
    }

    
    public void MaxAddHealth(int _add)
    {
        maxHealth += _add;
        currentHealth += _add;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
        lifeLookCamera.UpdateLifeSlider(currentHealth, maxHealth);
        lifeLookCamera.SetLife(currentHealth);
    }

    
    IEnumerator DeadAnimator()
    {
        SetState(PlayerState.Dead);
        isDead = true;
        AudioPlay(false);
        ResetBowWeaponPosAndQut();
        UIGame.Instance.ShowDangerEF();
        UIGame.Instance.ClearAddExperience();
        yield return new WaitForSeconds(2f);
        if (!GameManager.Instance.Win())
            GameManager.Instance.SetLose(true);
    }

    IEnumerator HurtAnimator()
    {
        SetState(PlayerState.GetHit);
        UIGame.Instance.ShowDangerEF();
        CameraFollow.Instance.Shake(0.2f, 0.5f);
        ResetBowWeaponPosAndQut();
        isHurted = true;
        arrowTime = 0f;
        lifeLookCamera.UpdateAttackSlider(0, 0);
        yield return new WaitForSeconds(0.05f);
        isHurted = false;
    }

    
    public void ShootArrowTimer()
    {
        SetState(PlayerState.Attack);
        AttackCircle.SetActive(true);
        
        if (arrowTime < arrowCoolTime && !isShoot)
        {
            arrowTime += Time.deltaTime * arrowSpeed;
            lifeLookCamera.UpdateAttackSlider(arrowTime, arrowCoolTime);
        }
        if (arrowTime >= arrowCoolTime)
        {
            arrowTime = 0;
            SpwanArrow();
        }
        lifeLookCamera.UpdateAttackSlider(arrowTime, arrowCoolTime);
    }

    
    public void SpwanArrow()
    {
        int arrowCount = 1;
        if (DoubleArrow)
        {
            arrowCount = 2;
        }
        SetState(PlayerState.Idle);
        isShoot = true;
        ResetBowWeaponPosAndQut();
        StartCoroutine(SpawnArrowEnumerator(arrowCount));
    }

    IEnumerator SpawnArrowEnumerator(int arrowCount)
    {
        
        if (arrowCount == 1)
        {
            
            SpawnSingleArrow(0f);
        }
        else
        {
            
            SpawnSingleArrow(0f);

            
            SpawnSingleArrow(-30f);

            
            SpawnSingleArrow(30f);
        }

        yield return new WaitForSeconds(0.4f);
        
        isShoot = false;
    }

    
    void SpawnSingleArrow(float angleOffset)
    {
        
        Quaternion baseRot = ArrowSpawnPoint.rotation;

        
        Quaternion offsetRot = Quaternion.AngleAxis(angleOffset, Vector3.up);

        
        Quaternion finalRot = baseRot * offsetRot;

        
        GameObject arrow = Instantiate(Arrow, ArrowSpawnPoint.position, finalRot);

        
        arrow.GetComponent<Arrow>().GetBuff(buff);
        arrow.GetComponent<Arrow>().SetDamage(ArrowDamage);
    }

    
    public void GetBuff(BuffType _buff)
    {
        if (buff != _buff)
        {
            buff = _buff;
            switch (_buff)
            {
                case BuffType.DoubleArrow:
                    DoubleArrow = true;
                    arrowSpeed = 1.5f;
                    break;
                case BuffType.AddSpeed:
                    arrowSpeed = 3f;
                    DoubleArrow = false;
                    break;
                case BuffType.Fire:
                    RestartBuff();
                    break;
                case BuffType.Water:
                    RestartBuff();
                    break;
            }
        }
    }

    public void RestartBuff()
    {
        DoubleArrow = false;
        arrowSpeed = 1.5f;
    }

    
    public void Respawn(Vector3 RespawnPos)
    {
        currentHealth = maxHealth;
        lifeLookCamera.SetLife(currentHealth);
        lifeLookCamera.UpdateLifeSlider();
        lifeLookCamera.UpdateAttackSlider(0, 0);
        ResetBowWeaponPosAndQut();
        isDead = false;
        isShoot = false;
        transform.position = RespawnPos;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        SetState(PlayerState.Idle);
        AttackCircle.SetActive(false);
        ClearEnemyLock();
        isLock = false;
        isHurted = false;
        AudioPlay(true);
        RestartBuff();
        DoubleArrow = false;
        buff = BuffType.None;
    }

    
    public void ChangeStepsMusic(int index)
    {
        if (audiosource.clip != Stepsounds[index - 1])
        {
            audiosource.clip = Stepsounds[index - 1];
            audiosource.Play();
        }
    }

    
    public void AudioPlay(bool _isPlay)
    {
        if (_isPlay != isPlay)
        {
            isPlay = _isPlay;
            if (isPlay)
                audiosource.Play();
            else
                audiosource.Stop();
        }
    }
}

