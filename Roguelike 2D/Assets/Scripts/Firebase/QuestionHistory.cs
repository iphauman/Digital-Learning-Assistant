using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionHistory
{
    public string Id;
    public string LastAttemptDate;
    public int MemoryStrength;
    public bool LastIsCorrect;

    public QuestionHistory(string id, string lastDate, int memory, bool correct)
    {
        Id = id;
        LastAttemptDate = lastDate;
        MemoryStrength = memory;
        LastIsCorrect = correct;
    }

    public override string ToString()
    {
        return "{ " + Id + ", " + LastAttemptDate + ", " + MemoryStrength + ", " + LastIsCorrect + " }";
    }
}
