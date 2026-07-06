using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool : SingleMonoBase<GameObjectPool>
{
    
    public Dictionary<string, List<GameObject>> all = new Dictionary<string, List<GameObject>>();
    
    public GameObject getobj(string key, GameObject obj)
    {
        
        if (!all.ContainsKey(key))
        {
            all.Add(key, new List<GameObject>());
        }
        
        GameObject clone = Find(key);
        
        if (clone == null)
        {
            clone = Instantiate(obj, this.transform);
            all[key].Add(clone);
        }
        
        clone.SetActive(true);
        return clone;
    }
    
    public GameObject Find(string key)
    {
        
        if (all.ContainsKey(key))
            foreach (GameObject item in all[key])
            {
                if (!item.activeSelf)
                    return item;
            }
        return null;
    }
    
    public void reback(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.parent = this.transform;
    }
    
    public void reback(GameObject obj, float time)
    {
        StartCoroutine(Delayback(obj, time));
    }
    IEnumerator Delayback(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
        obj.transform.parent = this.transform;
    }
}