using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startPos;
    public GameObject camera;
    public float effectAmount;
    
    
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    void FixedUpdate()
    {
        float distance = (camera.transform.position.x * effectAmount);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
