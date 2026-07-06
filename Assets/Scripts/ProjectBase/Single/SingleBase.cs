using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleBase<T> where T : new()
{
    private static T intance;

    public static T Instance
    {
        get
        {
            if (intance == null)
                intance = new T();
            return intance;
        }
    }
}
