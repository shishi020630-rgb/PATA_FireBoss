using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]

public class AppearBridge : MonoBehaviour
{
    public GameObject bridge;

    public GameObject bridgeWall;

    private MeshRenderer rend;

    private Material matInstance;

    public Material newMatInstance;

    public Image Bar;

    private float bridgeAppearCoolTimer = 1f;

    private float bridgeAppearTimer = 0f;

    public bool isAppearTipGoText = false;

    private AudioSource audioSource;

    void Awake()
    {
        rend = bridge.GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        matInstance = rend.material;
        
    }

    void OnTriggerStay(Collider other)
    {
        if (!GameManager.Instance.PlayGroundManager.IsTrigger)
            return;
        if (other.tag == "Player")
        {
            bridgeAppearTimer += Time.deltaTime;
            Bar.fillAmount = bridgeAppearTimer;
            if (audioSource.isPlaying == false)
                audioSource.Play();
            if (bridgeAppearTimer >= bridgeAppearCoolTimer)
            {
                bridgeAppearCoolTimer = 1f;
                audioSource.Stop();
                bridgeWall.SetActive(false);
                isAppearTipGoText = false;
                
                rend.material = newMatInstance;
                gameObject.SetActive(false);
                isAppearTipGoText = true;
                bridgeAppearTimer = 0f;
                AudioManager.Instance.PlaySound("BridgeComplete");
            }
            
        }
    }

    void Update()
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (!GameManager.Instance.PlayGroundManager.IsTrigger)
            return;
        if (other.tag == "Player")
        {
            bridgeAppearTimer = 0;
            
            Bar.fillAmount = 0;
            audioSource.Stop();
        }
    }
}
