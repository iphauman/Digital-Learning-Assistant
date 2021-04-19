using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    // Quiz UI
    public GameObject QuizUI;

    // Quiz UI - Elements
    public GameObject CharacterImage;
    public GameObject Description;
    public GameObject ChoiceA;
    public GameObject ChoiceB;
    public GameObject ChoiceC;
    public GameObject ChoiceD;

    public GameObject PurgeButton;

    // Quiz UI - Hint
    public GameObject HintUI;
    public GameObject HintDesc;
    public GameObject HintCloseButton;

    // Quiz UI - Result Image
    public GameObject correctImage;
    public GameObject incorrectImage;

    // Progress UI
    public GameObject ProgressUIManager;

    // temp store the current question
    Question question;
    public string answer;
    public string hint;


    // Invoker to be destory
    public GameObject Invoker;

    public void ChooseA() 
    {
        CheckCorrectness(ChoiceA.GetComponentInChildren<Text>().text);
    }

    public void ChooseB() 
    {
        CheckCorrectness(ChoiceB.GetComponentInChildren<Text>().text);
    }

    public void ChooseC() 
    {
        CheckCorrectness(ChoiceC.GetComponentInChildren<Text>().text);
    }

    public void ChooseD() 
    {
        CheckCorrectness(ChoiceD.GetComponentInChildren<Text>().text);
    }

    private void CheckCorrectness(string choice)
    {
        // if the choice is answer
        if (string.Compare(choice, answer) == 0)
        {
            StartCoroutine(ShowResultImage(true));
        }
        else
        {
            StartCoroutine(ShowResultImage(false));
        }
    }

    public IEnumerator ShowResultImage(bool correct)
    {
        GameObject image;
        if (correct)
        {
            // Debug.Log("Correct");

            // Remove question from local and update db
            FirebaseController.Instance.RemoveQuestion(question.id, Properties.Instance.BANK_NAME);
            image = correctImage;

            // update the number of remaining questions
            ProgressUIManager.GetComponent<ProgressUIManager>().UpdateRemainingQuestions(FirebaseController.Instance.Histories.Count.ToString());
        }
        else
        {
            // Debug.Log("Incorrect");

            // Put the question to the last position for next attempt
            FirebaseController.Instance.ReorderQuestion(question.id);
            image = incorrectImage;
        }

        image.SetActive(true);

        yield return new WaitForSeconds(1);

        image.SetActive(false);

        // Destroy the invoker (emeny) on the game environment
        Destroy(Invoker);

        // Hide the Quiz UI
        QuizUI.SetActive(false);
    }

    public void UpdateContent(GameObject invoker)
    {
        // Update invoker to be destory after complete the quiz process
        Invoker = invoker;

        // Retrieve a question from question bank
        question = FirebaseController.Instance.GetQuestion();
        // Debug.Log("UI should showing question: " + JsonConvert.SerializeObject(question, Formatting.Indented));

        // Update UI contents
        Description.GetComponent<Text>().text = question.desc;
        List<string> randomChoice = new List<string>();

        // Update Hint content
        HintDesc.GetComponent<Text>().text = question.explaination;

        // reset purge function 
        ResetButtonActive();
        PurgeButton.SetActive(true);

        // Update answer for choices
        answer = question.answer;

        // Assign description of optional choices
        randomChoice.Add(question.answer);
        foreach (string choice in question.fakeAnswer)
        {
            randomChoice.Add(choice);
        }

        ChoiceA.GetComponentInChildren<Text>().text = randomChoice[Random.Range(0, randomChoice.Count)];
        randomChoice.Remove(ChoiceA.GetComponentInChildren<Text>().text);

        ChoiceB.GetComponentInChildren<Text>().text = randomChoice[Random.Range(0, randomChoice.Count)];
        randomChoice.Remove(ChoiceB.GetComponentInChildren<Text>().text);

        ChoiceC.GetComponentInChildren<Text>().text = randomChoice[Random.Range(0, randomChoice.Count)];
        randomChoice.Remove(ChoiceC.GetComponentInChildren<Text>().text);

        ChoiceD.GetComponentInChildren<Text>().text = randomChoice[Random.Range(0, randomChoice.Count)];
        randomChoice.Remove(ChoiceD.GetComponentInChildren<Text>().text);
    }

    // Purge a choice
    public void PurgeChoice()
    {
        List<GameObject> choices = new List<GameObject>();
        choices.Add(ChoiceA);
        choices.Add(ChoiceB);
        choices.Add(ChoiceC);
        choices.Add(ChoiceD);

        List<GameObject> canPurge = new List<GameObject>();
        foreach (var choice in choices)
        {
            if (string.Compare(choice.GetComponentInChildren<Text>().text, answer) != 0 && choice.activeSelf == true)
            {
                canPurge.Add(choice);
            }
        }

        int rand = Random.Range(0, canPurge.Count);
        Debug.Log(canPurge[rand].GetComponentInChildren<Text>().text);
        canPurge[rand].SetActive(false);
        Debug.Log("Purged.");

        // Disable Purge Button
        PurgeButton.SetActive(false);
    }

    // Reset the Choices button due to Purge function which will disable button
    private void ResetButtonActive()
    {
        List<GameObject> choices = new List<GameObject>();

        choices.Add(ChoiceA);
        choices.Add(ChoiceB);
        choices.Add(ChoiceC);
        choices.Add(ChoiceD);

        foreach (var c in choices)
        {
            c.SetActive(true);
        }
    }

    public void ShowHint()
    {
        Debug.Log("Show hint.");

        // Show Hint UI
        HintUI.SetActive(true);
    }

    public void CloseHint()
    {
        Debug.Log("Close Hint UI");
        HintUI.SetActive(false);
    }

}
