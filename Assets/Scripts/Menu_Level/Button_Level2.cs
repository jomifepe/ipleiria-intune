using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Level2 : MonoBehaviour
{
    public void LoadLevel()
    {
        GameManager.Instance.LoadLevel(2);
    }
}
