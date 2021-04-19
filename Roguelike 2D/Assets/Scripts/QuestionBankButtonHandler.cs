using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionBankButtonHandler : MonoBehaviour
{
    // text
    public InputField desc;
    public InputField choiceA;
    public InputField choiceB;
    public InputField choiceC;
    public InputField choiceD;
    public InputField explaination;

    // toggles
    public ToggleGroup toggleGroup;
    public Toggle toggleA;
    public Toggle toggleB;
    public Toggle toggleC;
    public Toggle toggleD;

    // temp store questions
    public GameObject BankHolder;
    public QuestionBankCreationHolder holder;

    // show message
    public GameObject ResultPanel;
    public GameObject UploadMessage;

    // db connection
    public FirebaseController fc;

    // Timer for sequential logic
    readonly PromiseTimer Timer = new PromiseTimer();

    private void Start()
    {
        holder = BankHolder.GetComponent<QuestionBankCreationHolder>();
        fc = FirebaseController.Instance;
    }

    public void StoreQuestion()
    {
        Toggle activedToggle = null;
        // check which toggle is on, which will be the answer of the question
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            activedToggle = toggle;
        }

        // store all options and remove the answer
        Dictionary<string, string> optionalAnswers = new Dictionary<string, string>();
        optionalAnswers.Add("A", choiceA.text);
        optionalAnswers.Add("B", choiceB.text);
        optionalAnswers.Add("C", choiceC.text);
        optionalAnswers.Add("D", choiceD.text);

        // get the optional ansewrs
        string answer = "";
        string key = "";
        switch (activedToggle.name)
        {
            case "Toggle A":
                key = "A";
                answer = optionalAnswers[key];
                optionalAnswers.Remove(key);
                break;

            case "Toggle B":
                key = "B";
                answer = optionalAnswers[key];
                optionalAnswers.Remove(key);
                break;

            case "Toggle C":
                key = "C";
                answer = optionalAnswers[key];
                optionalAnswers.Remove(key);
                break;

            case "Toggle D":
                key = "D";
                answer = optionalAnswers[key];
                optionalAnswers.Remove(key);
                break;
        }

        // Check input is empty or not
        if (string.IsNullOrEmpty(desc.text)) { return; }
        if (string.IsNullOrEmpty(choiceA.text)) { return; }
        if (string.IsNullOrEmpty(choiceB.text)) { return; }
        if (string.IsNullOrEmpty(choiceC.text)) { return; }
        if (string.IsNullOrEmpty(choiceD.text)) { return; }

        // Add question for temp storing
        holder.AddQuestion(desc.text, answer, new List<string>(optionalAnswers.Values), explaination.text);

        // Clear the panel for next question
        desc.text = "";
        choiceA.text = "";
        choiceB.text = "";
        choiceC.text = "";
        choiceD.text = "";
        explaination.text = "";
    }

    public void CompleteCreation()
    {
        StoreQuestion();
        // StartCoroutine(fc.CreateQuestionBank(holder.BankName, holder.GetQuestionBank()));
        CreateQuestionBank(holder.BankName);
    }

    void CreateQuestionBank(string bankname)
    {
        UploadMessage.GetComponent<Text>().text = "Uploading...";
        Debug.Log("Start to upload question bank " + bankname);
        ResultPanel.SetActive(true);
        // yield return fc.CreateQuestionBank(holder.BankName, holder.GetQuestionBank());

        fc.CreateQuestionBank(bankname, holder.GetQuestionBank()).Then(result =>
        {
            if (result)
            {
                // Show uploaded message and return to login page
                UploadMessage.GetComponent<Text>().text = "Succeed to Upload Question bank [" + bankname + "]. Now return to Login Page.";
            }
            else
            {
                UploadMessage.GetComponent<Text>().text = "Failed to Upload Question bank [" + bankname + "]. Please Try again later";
            }
            

            Debug.Log("Wait 3 seconds to login page");
            return Timer.WaitFor(5f);
        }).Then(()=> 
        {
            Debug.Log("Load scene Login");
            SceneManager.LoadScene("Login");
        });
    }

    private void Update()
    {
        Timer.Update(Time.deltaTime);
    }

}
