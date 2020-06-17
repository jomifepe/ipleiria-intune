using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Level3 : MonoBehaviour
{
    public void LoadLevel()
    {
        GameManager.Instance.LoadLevel(3);
    }
}
