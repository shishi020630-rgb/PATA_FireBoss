using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipBall : MonoBehaviour
{
    public int Level = -1;

    
    public float explodeForce = 5f;

    
    public float upwardForce = 2f;

    
    public float dropRange = 2f;

    public GameObject[] EquipBallChildren;

    private bool isFllow = false;

    Rigidbody rb;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            GameManager.Instance.SetStop(true);
            UIController.Instance.AppearCard(Level);
            AudioManager.Instance.PlaySound("Get");
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb != null)
        {
            
            Vector2 offset2D = Random.insideUnitCircle * dropRange;
            Vector3 spawnPos = new Vector3(
                transform.position.x + offset2D.x,
                transform.position.y,
                transform.position.z + offset2D.y
            );

            Vector3 dir = new Vector3(offset2D.x, Random.Range(0.5f, 1f), offset2D.y).normalized;
            rb.AddForce(dir * explodeForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
            rb.velocity = new Vector3(rb.velocity.x, -4f, rb.velocity.z);
            
        }
    }

    void Update()
    {
        if (GameManager.Instance.Pause())
            return;
        
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            playerPos.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime * 30f);
            
        }
    }

    public void AppearCurrentChild(int level)
    {
        if (EquipBallChildren[Level] != null)
        {
            EquipBallChildren[Level].SetActive(true);
        }
    }

    public void SetLevel(int level)
    {
        Level = level;
    }
}
