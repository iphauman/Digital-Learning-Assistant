using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string id;
    public string desc;
    public string answer;
    public string[] fakeAnswer;
    public string explaination;

    public Question(string desc, string answer, string[] fakeAnswer, string explaination)
    {
        this.id = System.Guid.NewGuid().ToString();
        this.desc = desc;
        this.answer = answer;
        this.fakeAnswer = fakeAnswer;
        this.explaination = explaination;
    }
}
