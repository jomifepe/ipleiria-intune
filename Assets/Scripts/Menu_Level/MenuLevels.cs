﻿using UnityEngine;

public class MenuLevels : MonoBehaviour
{
    public void LoadLevel1()
    {
        GameManager.Instance.LoadLevel(1);
    }
    
    public void LoadLevel2()
    {
        GameManager.Instance.LoadLevel(2);
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
