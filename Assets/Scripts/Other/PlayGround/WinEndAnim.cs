using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WinEndAnim : MonoBehaviour
{
    
    public GameObject itemPrefab;                
    public Transform[] dropTargets;              

    
    public float popDuration = 0.35f;            
    public float popHeight = 1.2f;               
    public float startScale = 0.2f;              
    public float targetScale = 1f;               

    
    public float fallDelay = 0.15f;              
    public float fallDuration = 0.5f;            
    public float dropRadius = 1.0f;              

    
    public bool useRandomRotationOnSpawn = true; 
    public bool destroyIfNoPrefab = true;        

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        for (int i = 0; i < 7; i++)
        {
            TriggerDrop();
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    public void TriggerDrop()
    {
        GameObject go = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        if (useRandomRotationOnSpawn)
            go.transform.rotation = Random.rotation;
        go.transform.parent = transform;

        StartCoroutine(PopThenDropRoutine(go));
    }

    IEnumerator PopThenDropRoutine(GameObject item)
    {
        Transform t = item.transform;

        
        t.localScale = Vector3.one;
        t.localScale = Vector3.one * startScale;
        
        Vector3 origin = t.position;
        Vector3 popTargetPos = origin + Vector3.up * popHeight;
        t.position = origin;

        float elapsed = 0f;
        
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / popDuration);
            float ease = 1f - Mathf.Pow(1f - k, 3f);
            t.position = Vector3.Lerp(origin, popTargetPos, ease);
            t.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, ease);

            yield return null;
        }

        
        t.position = popTargetPos;
        t.localScale = Vector3.one * targetScale;

        
        if (fallDelay > 0f) yield return new WaitForSeconds(fallDelay);

        
        Vector3 finalPos = ChooseRandomDropPosition();

        
        elapsed = 0f;
        Vector3 startPos = t.position;
        float peakOffset = 0.4f; 
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / fallDuration);
            
            float ease = k * k;
            
            float height = Mathf.Sin(Mathf.PI * k) * peakOffset;
            
            Vector3 flat = Vector3.Lerp(startPos, finalPos, ease);
            t.position = new Vector3(flat.x, flat.y + height, flat.z);
            yield return null;
        }

        t.position = finalPos;

        
    }

    void Update()
    {
        
    }

    Vector3 ChooseRandomDropPosition()
    {
        
        if (dropTargets != null && dropTargets.Length > 0)
        {
            int idx = Random.Range(0, dropTargets.Length);
            return dropTargets[idx].position;
        }

        
        Vector2 circle = Random.insideUnitCircle * dropRadius;
        Vector3 pos = transform.position + new Vector3(circle.x, 0f, circle.y);
        return pos;
    }
}