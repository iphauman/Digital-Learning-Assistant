using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBankButtonHandler : MonoBehaviour
{
    public Text bankname;
    public Text debug;

    public GameObject QuestionInputPanel;
    public GameObject CreateBankPanel;

    public QuestionBankCreationHolder holder;

    public void SetBankName()
    {
        if (!string.IsNullOrEmpty(bankname.text))
        {
            Print("bankname = " + bankname.text);
            holder.GetComponent<QuestionBankCreationHolder>().BankName = bankname.text;
            Print("bankname in holder = " + holder.GetComponent<QuestionBankCreationHolder>().BankName);
            QuestionInputPanel.SetActive(true);
            CreateBankPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Bankname is empty!");
        }
    }

    public void Print(string message)
    {
        debug.text = message + "\n" + debug.text;
    }
}
