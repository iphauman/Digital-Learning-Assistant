using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonToMain : MonoBehaviour
{
    public void ReturnMain()
    {
        SceneManager.LoadScene("Login");
    }
}
