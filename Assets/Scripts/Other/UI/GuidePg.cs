using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuidePg : MonoBehaviour
{
    
    public List<GameObject> Guides = new List<GameObject>();

    

    public GameObject OneEndGuide;

    

    private bool isOneEnd = false;

    public int index = -1;

    private Animator anim;

    void Start()
    {
        
        foreach (GameObject guide in Guides)
        {
            guide.SetActive(false);
        }

        if (OneEndGuide != null)
        {
            OneEndGuide.SetActive(false);
        }


        
    }

    public void PlayAnimatorEndStartGame(int temp)
    {
        StartCoroutine(AnimatorEndStartGame(temp));
    }

    public void PlayAnimatorOneEnd()
    {
        StartCoroutine(AnimatorOneEnd());
    }

    IEnumerator AnimatorOneEnd()
    {
        anim = OneEndGuide.GetComponent<Animator>();
        
        yield return new WaitForSeconds(0f);
        GameManager.Instance.SetStop(false);
        
        OneEndGuide.gameObject.SetActive(false);

    }

    IEnumerator AnimatorEndStartGame(int temp)
    {
        anim = Guides[temp].GetComponent<Animator>();
        
        yield return new WaitForSeconds(0f);
        GameManager.Instance.SetStop(false);
        
        Guides[temp].gameObject.SetActive(false);
    }

    public void ShowBtn()
    {
        
    }

    public void ShowGuide()
    {
        index = GameManager.Instance.EnemyManager.GetLevel();
        if (Guides[index].gameObject != null)
        {
            Guides[index].SetActive(true);
        }

        
    }

    public void ShowOneEndGuide()
    {
        if (isOneEnd) return;
        GameManager.Instance.SetStop(true);
        
        OneEndGuide.SetActive(true);
        isOneEnd = true;
        
    }

    public void ShowOneEndBtn()
    {
        
    }
}
