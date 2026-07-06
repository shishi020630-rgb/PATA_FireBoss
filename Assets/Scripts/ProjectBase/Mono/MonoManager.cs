using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MonoManager : SingleBase<MonoManager>
{
    public MonoController monoController;

    public MonoManager()
    {
        GameObject obj = new GameObject("monoController");
        monoController = obj.AddComponent<MonoController>();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        monoController.AddUpdateListener(fun);
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        monoController.RemoveUpdateListener(fun);
    }

    
    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return monoController.StartCoroutine(enumerator); ;
    }
}