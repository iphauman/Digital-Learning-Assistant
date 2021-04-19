using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseRestTester : MonoBehaviour
{
    // Start is called before the first frame update
    FirebaseRest fb;

    bool GotQuestionBank = false;

    async void Start()
    {
        fb = FirebaseRest.Instance;

        // === Create question bank ===

        /*
        List<Question> bank = new List<Question>();
        bank.Add(new Question("Is Python case sensitive when dealing with identifiers?"
            , "yes", new string[] { "No", "machine dependent", "none of the mentioned" }));
        fb.CreateQuestionBank("Python", bank);
        fb.AddQuestionToBank("Python", new Question("What is the maximum possible length of an identifier?"
            , "any length", new string[] { "31 characters", "63 characters", "127 chrarcters" }));
        */

        // === Retrieve data ===

        StartCoroutine(RetrieveQuestionBank());
    }

    IEnumerator RetrieveQuestionBank()
    {
        Debug.Log("Getting Bank List");
        yield return fb.RetrieveBankList();

        Debug.Log("Retrieving questions from bank");
        yield return fb.RetrieveQuestionBank("Python");
        // yield return new WaitUntil(() => fb.GotBank == true);

        Debug.Log("Retrieving History");
        yield return fb.RetrieveHistory("David", "Python");
        if (fb.QuestionHistory == null)
        {
            Debug.Log("No history");
            fb.QuestionHistory = new Dictionary<string, QuestionHistory>();
        }

        Debug.Log("Updating History");

        // compare the history to add new record if new question encounter
        foreach (string id in fb.QuestionBank.Keys)
        {
            if (!fb.QuestionHistory.ContainsKey(id))
            {
                // get current date
                DateTime dt = DateTime.Now;
                string currentDate = dt.ToShortDateString().ToString();
                // int dayPassed = dt.Subtract(lastAttempt).Days;

                fb.QuestionHistory.Add(id, new QuestionHistory(id, null, 0, false));
            }
        }

        List<QuestionHistory> history = new List<QuestionHistory>();
        foreach (QuestionHistory his in fb.QuestionHistory.Values)
        {
            history.Add(his);
        }

        // fb.UpdateHistory("David", "Python", history);

        GotQuestionBank = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GotQuestionBank)
        {
            // do something
            GotQuestionBank = false;
        }
    }
}
