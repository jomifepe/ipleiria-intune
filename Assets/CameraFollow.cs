using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform topLimitTransform;
    [SerializeField] private Transform rightLimitTransform;
    [SerializeField] private Transform bottomLimitTransform;
    [SerializeField] private Transform leftLimitTransform;
    [SerializeField] private float smoothTime = 0.5f;

    private Camera myCamera;
    private float zOffset;
    private Vector3 cameraVelocity;
    private Vector3 lastOffsetPosition = Vector3.zero;
    private Coroutine lastShakeCoroutine;
    private float topLimit, rightLimit, bottomLimit, leftLimit;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        zOffset = transform.position.z;
        myCamera = GetComponent<Camera>();
        SetCameraLimits();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = targetTransform.position;
        targetPosition.z = zOffset;

        // targetPosition.x = Mathf.Clamp(targetPosition.x, leftLimit, rightLimit);
        targetPosition.y = Mathf.Clamp(targetPosition.y, bottomLimit, topLimit);
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref cameraVelocity,
            smoothTime
        );            
    }

    public void SetTarget(Transform newTargetTransform)
    {
        targetTransform = newTargetTransform;
    }

    private IEnumerator DoShake(float duration, float range)
    {
        while (duration > 0f)
        {
            lastOffsetPosition = Random.insideUnitSphere * range;
            lastOffsetPosition.z = 0f;
            transform.localPosition += lastOffsetPosition;

            if (duration < 0.5f)
            {
                range *= 0.90f;
            }
            
            duration -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition -= lastOffsetPosition;
    }

    public void Shake(float duration, float range)
    {
        transform.localPosition -= lastOffsetPosition;
        if (lastShakeCoroutine != null)
        {
            StartCoroutine(DoShake(duration, range));
        }
        lastShakeCoroutine = StartCoroutine(DoShake(duration, range));
    }

    private void SetCameraLimits()
    {
        float halfHeight = myCamera.orthographicSize;
        float halfWidth = halfHeight * myCamera.aspect;

        if (topLimitTransform != null) topLimit = topLimitTransform.position.y - halfHeight;
        if (rightLimitTransform != null) rightLimit = rightLimitTransform.position.x - halfWidth;
        if (bottomLimitTransform != null) bottomLimit = bottomLimitTransform.position.y + halfHeight;
        if (leftLimitTransform != null) leftLimit = leftLimitTransform.position.x + halfWidth;
    }

    public void SetTopLimit(Transform top)
    {
        topLimitTransform = top;
        SetCameraLimits();
    }
    
    public void SetRightLimit(Transform right)
    {
        rightLimitTransform = right;
        SetCameraLimits();
    }
    
    public void SetBottomLimit(Transform bottom)
    {
        bottomLimitTransform = bottom;
        SetCameraLimits();
    }
    
    public void SetLeftLimit(Transform left)
    {
        leftLimitTransform = left;
        SetCameraLimits();
    }
}
