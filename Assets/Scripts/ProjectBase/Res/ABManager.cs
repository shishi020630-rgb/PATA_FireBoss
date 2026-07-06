using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : MonoBehaviour
{
    
    public static ABManager instance;

    
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    AssetBundle mainAB;
    AssetBundleManifest manifest;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    
    public void LoadAB(string abName)
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(Config.ABPath + "AB");
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        
        string[] dirs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < dirs.Length; i++)
        {
            if (!abDic.ContainsKey(dirs[i]))
            {
                AssetBundle manifestab = AssetBundle.LoadFromFile(Config.ABPath + dirs[i]);
                abDic.Add(dirs[i], manifestab);
            }
        }

        
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(Config.ABPath + abName);
            abDic.Add(abName, ab);
        }
    }

    #region 
    public Object LoadRes(string abName, string resName)
    {

        LoadAB(abName);
        
        Object obj = abDic[abName].LoadAsset(resName);
        if (obj is GameObject)
        {
            return Instantiate(obj);
        }
        
        return obj;
    }

    
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB(abName);

        Object obj = abDic[abName].LoadAsset(resName, type);
        if (obj is GameObject)
        {
            return Instantiate(obj);
        }
        return obj;
    }

    
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        LoadAB(abName);

        T obj = abDic[abName].LoadAsset<T>(resName);

        if (obj is GameObject)
        {
            return Instantiate(obj);
        }
        return obj;
    }
    #endregion

    #region 
    public void LoadREsAsync(string abName, string resName, UnityAction<Object> callback)
    {
        StartCoroutine(RellyLoadAsync(abName, resName, callback));
    }

    IEnumerator RellyLoadAsync(string abName, string resName, UnityAction<Object> callback)
    {
        LoadAB(abName);
        
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;

        
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }

    
    public void LoadREsAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        StartCoroutine(RellyLoadAsync(abName, resName, type, callback));
    }

    IEnumerator RellyLoadAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        LoadAB(abName);
        
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;

        
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }

    
    public void LoadREsAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(RellyLoadAsync<T>(abName, resName, callback));
    }

    IEnumerator RellyLoadAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object
    {
        LoadAB(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;

        
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset) as T);
        }
        else
        {
            callback(abr.asset as T);
        }
    }
    #endregion

    #region 
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }
    
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
    #endregion
}