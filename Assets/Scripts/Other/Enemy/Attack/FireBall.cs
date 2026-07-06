using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    
    private float speed = 20f;        
    private float lifeTime = 2f;      

    private BuffType buffer = BuffType.None;

    private int damage = 0;

    void Start()
    {
        Invoke("Dead", lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.Player.Hurt(damage);
            
        }
    }

    void Update()
    {
        
        transform.position += transform.forward * speed * Time.deltaTime;

    }
    private void Dead()
    {
        Destroy(gameObject);
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }
}
