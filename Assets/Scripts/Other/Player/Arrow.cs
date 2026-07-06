using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    
    private float speed = 20f;        
    private float lifeTime = 2f;      

    private BuffType buffer = BuffType.None;

    
    public GameObject[] Arrows;

    private int damage = 0;

    public bool isFire = false;

    public bool isWater = false;

    void Start()
    {
        Invoke("Dead", lifeTime);
    }

    void Update()
    {
        
        transform.position += transform.forward * speed * Time.deltaTime;

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.TryGetComponent<Enemy>(out var enemy))
        {
            AudioManager.Instance.PlaySound("ShootArrow");
            if (collision.transform.GetComponent<Spider>() != null)
            {
                int enemyGetHurtEF = 0;
                if (isFire)
                {
                    damage += 20;
                    enemyGetHurtEF = 1;
                }
                else if (isWater)
                {
                    damage += 10;
                    enemyGetHurtEF = 2;
                }

                enemy.Hurt(damage, enemyGetHurtEF);
            }
            else if (collision.transform.GetComponent<SpiderMother>() != null)
            {
                int enemyGetHurtEF = 0;
                if (isFire)
                {
                    damage *= 2;
                    enemyGetHurtEF = 1;
                }
                else if (isWater)
                {
                    damage -= 10;
                    enemyGetHurtEF = 2;
                }

                enemy.Hurt(damage, enemyGetHurtEF);
            }
            else if (collision.transform.GetComponent<FireBoss>() != null)
            {
                int enemyGetHurtEF = 0;
                if (isFire)
                {
                    damage -= 10;
                    enemyGetHurtEF = 1;
                }
                else if (isWater)
                {
                    damage += 50;
                    enemyGetHurtEF = 2;
                }

                enemy.Hurt(damage, enemyGetHurtEF);
            }
            this.GetComponent<Collider>().enabled = false;
            
        }
    }

    private void Dead()
    {
        Destroy(gameObject);
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }

    public void GetBuff(BuffType _buffer)
    {
        buffer = _buffer;
        switch (buffer)
        {
            case BuffType.Fire:
                isFire = true;
                Arrows[1].SetActive(true);
                break;
            case BuffType.Water:
                isWater = true;
                Arrows[2].SetActive(true);
                break;
            case BuffType.DoubleArrow:
                Arrows[0].SetActive(true);
                break;
            case BuffType.AddSpeed:
                Arrows[0].SetActive(true);
                break;
            case BuffType.None:
                Arrows[0].SetActive(true);
                break;
        }
    }
}
