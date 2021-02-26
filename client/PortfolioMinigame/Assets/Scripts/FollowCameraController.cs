using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraController : MonoBehaviour
{
    [SerializeField] private Vector3 initialOffset;
    public Transform targetTransform;
    public Transform carTransform;
    public float roughness;
    public CameraFollowMode followMode;

    void Start()
    {
        initialOffset = transform.position - carTransform.position;
        // initialOffset = Vector3.zero;
    }

    void Update()
    {
        if (followMode == CameraFollowMode.TPS)
        {
            transform.position =
                Vector3.Lerp(transform.position, targetTransform.position, roughness * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, targetTransform.forward, roughness * Time.deltaTime);
        }
        else if (followMode == CameraFollowMode.TOP_DOWN)
        {
            // transform.position =
            //     Vector3.Lerp(transform.position, carTransform.position + initialOffset, roughness * Time.deltaTime);
            transform.position = carTransform.position + initialOffset;
            transform.LookAt(carTransform);
        }
    }
}

public enum CameraFollowMode
{
    TOP_DOWN,
    TPS
}