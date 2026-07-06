using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleMonoBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
