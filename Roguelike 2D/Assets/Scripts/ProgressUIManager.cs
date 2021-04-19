using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIManager : MonoBehaviour
{
    public Text ProgressText;
    public void UpdateRemainingQuestions(string remaining)
    {
        ProgressText.text = "Remaining: " + remaining;
    }
}
