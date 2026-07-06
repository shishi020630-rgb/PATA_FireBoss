using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIPanelBase : MonoBehaviour
{
    
    private Dictionary<string, List<UIBehaviour>> controllerDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindControlInChildren<Button>();
        FindControlInChildren<Text>();
        FindControlInChildren<TextMeshProUGUI>();
        FindControlInChildren<Toggle>();
        FindControlInChildren<ScrollRect>();
        FindControlInChildren<Slider>();
        FindControlInChildren<Image>();
    }

    
    public T GetControl<T>(string controllname) where T : UIBehaviour
    {
        if (controllerDic.ContainsKey(controllname))
        {
            for (int i = 0; i < controllerDic[controllname].Count; i++)
            {
                if (controllerDic[controllname][i] is T)
                    return controllerDic[controllname][i] as T;
            }
        }

        return null;
    }

    
    private void FindControlInChildren<T>() where T : UIBehaviour
    {
        T[] contoller = this.GetComponentsInChildren<T>();
        for (int i = 0; i < contoller.Length; i++)
        {
            if (controllerDic.ContainsKey(contoller[i].gameObject.name))
                controllerDic[contoller[i].gameObject.name].Add(contoller[i]);
            else
                controllerDic.Add(contoller[i].gameObject.name, new List<UIBehaviour>() { contoller[i] });
        }
    }

    
    public virtual void ShowMe()
    {
        gameObject.SetActive(true);
    }

    
    public virtual void HideMe()
    {
        gameObject.SetActive(false);
    }
}
