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
        var position = camera.transform.position;
        float temp = (position.x * (1 - effectAmount));
        float distance = (position.x * effectAmount);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += temp;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }
}
