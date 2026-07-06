using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class SceneMgr : SingleBase<SceneMgr>
{
    
    public void LoadScene(string name, UnityAction action)
    {
        
        SceneManager.LoadScene(name);
        
        action();
    }

    
    public void LoadSceneAsyn(string name, UnityAction action)
    {
        
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, action));
    }

   
    public IEnumerator ReallyLoadSceneAsyn(string name, UnityAction action)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        
        while (!async.isDone)
        {
            
            EventCenter.Instance.EventTrigger("������", async.progress);
            
            yield return async.progress;
        }
        yield return async;
        
        action();
    }
}
