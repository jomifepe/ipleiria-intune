using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    private float zOffset;
    private Vector3 cameraVelocity;
    [SerializeField]
    private float smoothTime = 0.5f;

    private void Awake()
    {
        zOffset = transform.position.z;
    }

    private void LateUpdate()
    {
        if (targetTransform != null)
        {
            Vector3 targetPosition = targetTransform.position;
            targetPosition.z = zOffset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref cameraVelocity,
                smoothTime
                );

        }
    }
}
