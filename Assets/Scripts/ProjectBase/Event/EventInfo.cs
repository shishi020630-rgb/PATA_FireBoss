using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IEventInfo { }

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


public class EventCenter : SingleBase<EventCenter>
{
    
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    
    public void EventListenner<T>(string eventName, UnityAction<T> action)
    {
        
        if (eventDic.ContainsKey(eventName))
        {
            
            (eventDic[eventName] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic[eventName] = new EventInfo<T>(action);
        }
    }

    
    public void EventListenner(string eventName, UnityAction action)
    {
        
        if (eventDic.ContainsKey(eventName))
        {
            
            (eventDic[eventName] as EventInfo).actions += action;
        }
        else
        {
            eventDic[eventName] = new EventInfo(action);
        }
    }

    
    public void EventTrigger<T>(string eventName, T info)
    {
        if (eventDic.ContainsKey(eventName))
        {
            
            if ((eventDic[eventName] as EventInfo<T>).actions != null)
                (eventDic[eventName] as EventInfo<T>).actions(info);
        }
    }

    
    public void EventTrigger(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            
            if ((eventDic[eventName] as EventInfo).actions != null)
                (eventDic[eventName] as EventInfo).actions();//ִ��ί�к���
        }
    }

    
    public void RemoveEvent<T>(string eventName, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions -= action;
        }
    }

    
    public void RemoveEvent(string eventName, UnityAction action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions -= action;
        }
    }
}