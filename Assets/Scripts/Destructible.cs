using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    private Animator animator;
    private bool isIntact = true;
    
    private string AnimDestruct = "Destruct";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Destroy()
    {
        if (isIntact)
        {
            isIntact = false;
            animator.SetTrigger(AnimDestruct);
            Invoke(nameof(DestroyObject), 3);
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
