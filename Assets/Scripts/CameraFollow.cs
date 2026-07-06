using System.Collections;
using UnityEngine;

public class CameraFollow : SingleMonoBase<CameraFollow>
{
    public Transform target;          
    public float smoothSpeed = 5f;    

    private Vector3 offset;           

    private bool isShaking = false;   
    private Vector3 shakeOffset = Vector3.zero; 

    float CameraX = 0;

    public float MaxCameraZ3 = 129.4f;

    public float MaxCameraZ = -12f;

    public float MaxCameraZ2 = 52.4f;

    private Camera cam;

    
    public bool isFllow = false;

    

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("");
            return;
        }

        cam = GetComponent<Camera>();

       
        offset = transform.position - target.position;
        CameraX = offset.x;
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (GameManager.Instance.Pause() && !isFllow) return;

        
        Vector3 desiredPosition = target.position + offset + shakeOffset;
        if (!isFllow)
        {
            desiredPosition.x = CameraX;
        }
        transform.position = Vector3.Lerp(transform.position,
        desiredPosition,
        smoothSpeed * Time.deltaTime);
        if (!isFllow)
        {
            if (GameManager.Instance.EnemyManager.GetLevel() == 0)
            {
                if (transform.position.z >= MaxCameraZ)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, MaxCameraZ);
                }
            }
            if (GameManager.Instance.EnemyManager.GetLevel() == 1)
            {
                if (transform.position.z >= MaxCameraZ2)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, MaxCameraZ2);
                }
            }
            else if (GameManager.Instance.EnemyManager.GetLevel() == 2)
            {
                if (transform.position.z >= MaxCameraZ3)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, MaxCameraZ3);
                }
            }
        }


        
    }

    
    public void Shake(float duration = 0.2f, float magnitude = 0.15f)
    {
        if (!isShaking)
            StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        
        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}
