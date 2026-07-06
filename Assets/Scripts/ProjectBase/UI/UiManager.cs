using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}


public class UiManager : SingleBase<UiManager>
{
    
    public Dictionary<string, UIPanelBase> panelDic = new Dictionary<string, UIPanelBase>();

    
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;
    private Camera camera;

    
    public RectTransform canvas;
    public Canvas canvas2;

    public UiManager()
    {
        
        GameObject obj = ResManager.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        canvas2 = canvas.GetComponent<Canvas>();
        GameObject.DontDestroyOnLoad(obj);

        
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");

        
        obj = ResManager.Instance.Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }

    
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return this.bot;

            case E_UI_Layer.Mid:
                return this.mid;

            case E_UI_Layer.Top:
                return this.top;

            case E_UI_Layer.System:
                return this.system;
        }
        return null;
    }

    
    public void ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : UIPanelBase
    {
        if (panelDic.ContainsKey(panelName))
        {
            
            panelDic[panelName].transform.SetAsFirstSibling();
            panelDic[panelName].ShowMe();
            
            if (callBack != null)
                callBack(panelDic[panelName] as T);
            
            return;
        }

        ResManager.Instance.LoadResAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            
            Transform father = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = mid;
                    break;

                case E_UI_Layer.Top:
                    father = top;
                    break;

                case E_UI_Layer.System:
                    father = system;
                    break;
            }
            
            obj.transform.SetParent(father);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            
            T panel = obj.GetOrAddComponent<T>();

            
            panel.transform.SetAsFirstSibling();
            panel.ShowMe();

            
            panelDic.Add(panelName, panel);

            
            if (callBack != null)
                callBack(panel);
        });
    }

    
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
        }
    }

    
    public void DeletePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    
    public T GetPanel<T>(string name) where T : UIPanelBase
    {
        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;
        return null;
    }

    
    public void SetUICamera(Camera cam)
    {
        canvas2.worldCamera = cam;
        canvas2.planeDistance = 10;
    }
}