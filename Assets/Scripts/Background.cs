using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    void Start()
    {
        int index = SpriteRenderingOrderManager.Instance.GetBackgroundOrderInLayer();
        GetComponent<SpriteRenderer>().sortingOrder = index;
    }
}
