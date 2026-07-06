using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MonoController : MonoBehaviour
{
    public event UnityAction updataEvent;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (updataEvent != null)
            updataEvent();
    }

    
    public void AddUpdateListener(UnityAction fun)
    {
        updataEvent += fun;
    }

    
    public void RemoveUpdateListener(UnityAction fun)
    {
        updataEvent -= fun;
    }
}
