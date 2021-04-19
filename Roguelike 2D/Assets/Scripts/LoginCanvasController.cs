using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginCanvasController : MonoBehaviour
{
    public Toggle Teacher;
    public Toggle Student;
    public InputField username;
    public FirebaseController fc;
    void Start()
    {
        fc = FirebaseController.Instance;
    }

    public void Setup()
    {
        // Debug.Log(username.text);
        fc.Username = username.text;
        fc.InitReady = false;
        if (Teacher.isOn)
        {
            // Load Question Bank Creator
            SceneManager.LoadScene("QuestionBankCreationScene");
        }

        if (Student.isOn)
        {
            // Load Game Level
            SceneManager.LoadScene("GamingScene");
        }
    }

}
