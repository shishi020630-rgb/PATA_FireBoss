using UnityEngine;
using System.Collections;

public class TerrainRotator : MonoBehaviour
{
    
    public float rotateInterval = 5f;     
    public float rotateDuration = 0.5f;   

    private Quaternion originalRotation;  
    private bool isRotating = false;

    private bool bossIsDead = false;

    private bool isAppear = false;

    public void SetBossIsDead()
    {
        bossIsDead = true;
    }

    void OnEnable()
    {
        originalRotation = transform.rotation;
        StartCoroutine(RotateRoutine());
    }

    
    IEnumerator RotateRoutine()
    {
        while (true)
        {
            if (!bossIsDead)
            {
                yield return new WaitForSeconds(rotateInterval - 1);
                if (!isAppear)
                {
                    isAppear = true;
                    UIGame.Instance.ShowAndHidePlayGoundTipTitle(true);
                    yield return new WaitForSeconds(1.2f);
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                }

                if (!isRotating)
                    yield return StartCoroutine(Rotate180());
            }
            else
            {
                UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
                StopAllCoroutines();
                break;
            }
        }
    }

    
    IEnumerator Rotate180()
    {
        if (!GameManager.Instance.Pause())
        {
            UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
            isRotating = true;

            Quaternion startRot = transform.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(0, 90, 0);

            float t = 0f;
            while (t < rotateDuration)
            {
                t += Time.deltaTime;
                float lerp = t / rotateDuration;

                transform.rotation = Quaternion.Lerp(startRot, endRot, lerp);

                yield return null;
            }

            transform.rotation = endRot;
            isRotating = false;
        }
    }

    
    public void ResetRotation()
    {
        StopAllCoroutines();
        transform.rotation = originalRotation;
        UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
        isAppear = false;

        
        StartCoroutine(RotateRoutine());
    }

    
    public void LoseAndStopRotation()
    {
        StopAllCoroutines();
        isAppear = false;
        transform.rotation = originalRotation;
        UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
    }

    
    public void WinAndStopRotation()
    {
        StopAllCoroutines();
        UIGame.Instance.ShowAndHidePlayGoundTipTitle(false);
    }
}
