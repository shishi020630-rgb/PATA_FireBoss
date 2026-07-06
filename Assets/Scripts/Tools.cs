using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    
    private static Dictionary<char, string> artNums = new Dictionary<char, string>
    {
        {'0', "<sprite=0>"},
        {'1', "<sprite=1>"},
        {'2', "<sprite=2>"},
        {'3', "<sprite=3>"},
        {'4', "<sprite=4>"},
        {'5', "<sprite=5>"},
        {'6', "<sprite=6>"},
        {'7', "<sprite=7>"},
        {'8', "<sprite=8>"},
        {'9', "<sprite=9>"},
        {'L', "<sprite=10>"},
        {'/', "<sprite=11>"},
    };
    public static string NumToArtNum(string num)
    {
        string str = "";
        foreach (char s in num)
        {
            str += artNums[s];
        }
        return str;
    }

    
    public static string HurtNum()
    {
        string value = Random.Range(1111, 9999) + "M";
        return NumToArtNum(value);
    }

    
    public static class ScreenCircleHelper
    {
       
        public static Vector3 GetPointOnCircleEdge(Transform objTransform, bool is3D, float radiusPixels, float angleDegrees, Camera camera = null)
        {
            if (objTransform == null)
            {
                Debug.LogError("");
                return Vector3.zero;
            }

            
            Camera cam = camera ?? Camera.main;
            if (is3D && cam == null)
            {
                Debug.LogError("");
                return Vector3.zero;
            }

            
            Vector3 objScreenPos;
            if (cam != null)
            {
                
                objScreenPos = cam.WorldToScreenPoint(objTransform.position);
            }
            else
            {
                
                objScreenPos = Vector3.zero;
            }

            
            float rad = angleDegrees * Mathf.Deg2Rad;
            float dx = Mathf.Cos(rad) * radiusPixels;
            float dy = Mathf.Sin(rad) * radiusPixels;

            
            Vector3 resultScreen = new Vector3(objScreenPos.x + dx, objScreenPos.y + dy, 0f);

            if (!is3D)
            {
                
                return resultScreen;
            }

            
            resultScreen.z = objScreenPos.z; 
            Vector3 worldPoint = cam.ScreenToWorldPoint(resultScreen);

            return worldPoint;
        }

        
        public static Vector3 GetPointOnCircleEdge(GameObject obj, bool is3D, float radiusPixels, float angleDegrees, Camera camera = null)
        {
            if (obj == null) return Vector3.zero;
            return GetPointOnCircleEdge(obj.transform, is3D, radiusPixels, angleDegrees, camera);
        }
    }

}