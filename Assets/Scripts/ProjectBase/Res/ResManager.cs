using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ResManager : SingleBase<ResManager>
{
    
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else
            return res;
    }

    
    public void LoadResAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        
        MonoManager.Instance.StartCoroutine(ReallyLoadAsyn(name, callback));
    }

    
    private IEnumerator ReallyLoadAsyn<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(name);
        yield return request;
        if (request.asset is GameObject)
        {
            if(callback != null) callback(GameObject.Instantiate(request.asset) as T);
        }
        else
        {
            if (callback != null) callback(request.asset as T);
        }
    }
}