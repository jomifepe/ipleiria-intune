using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsButtonHandler : MonoBehaviour
{
    public void OpenOptionMenu()
    {
        SceneManager.LoadScene("Option_inMenu");
    }
}
