using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class QuestionBankCreationHolder : MonoBehaviour
{
    public string BankName { get; set; }
    public List<Question> questions;

    private void Awake()
    {
        questions = new List<Question>();
    }

    // Add question to the temp bank holder.
    // Parameters:
    //      desc: Description of a question
    //      answer: The answer of the question
    //      optionals: The other possible answers which not the correct one
    public void AddQuestion(string desc, string answer, List<string> optionals, string explain)
    {
        // Debug
        StringBuilder sb = new StringBuilder();
        sb.Append("Desc: " + desc);
        sb.Append(", " + "Answer: " + answer);
        foreach (string str in optionals)
        {
            sb.Append(", " + "Optional: " + str);
        }

        Debug.Log(sb.ToString());

        questions.Add(new Question(desc, answer, optionals.ToArray(), explain));

        Debug.Log("Total questions: " + questions.Count);
    }

    public List<Question> GetQuestionBank()
    {
        return questions;
    }
}
