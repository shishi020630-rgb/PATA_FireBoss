using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipLookCamera : MonoBehaviour
{
    private Camera cam;

    
    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    
    private void Update()
    {
        
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                         cam.transform.rotation * Vector3.up);
    }
}
