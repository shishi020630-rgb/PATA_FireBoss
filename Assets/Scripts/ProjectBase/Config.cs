using UnityEngine;

public class Config
{
    
    public static string ABPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "AB/";

    
    public static string petPrefabPath = "Prefab/Pet/";

    
    public static int centrePetID = 1;
    
    public static int addPetID = 1001;
    
    public static int[] enemyIDs = { 2, 2002, 2002, 3002, 3002 };
    
    public static float defaultAttackDis = 1.5f;
    
    public static float rotateSpeed = 5;
    
    public static float moveSpeed = 2;
    
    public static float AttackSpace = 1.5f;
    
    public static float chargeAttackDis = 5f;
    
    public static int charge = 100;
    
    public static int oneCharge = 10;
}
