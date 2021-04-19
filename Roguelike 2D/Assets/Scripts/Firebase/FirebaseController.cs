using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using RSG;

[System.Serializable]
public class FirebaseController : MonoBehaviour
{
    private static FirebaseController _instance;
    public static FirebaseController Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = FindObjectOfType<FirebaseController>();
            if (_instance != null)
            {
                return _instance;
            }
            CreateDefault();
            return _instance;
        }
    }

    FirebaseRest fb;
    
    // Data
    public string Username { get; set; }
    public List<Bank> Banks { get; set; }
    public List<QuestionHistory> Histories { get; set; }
    public Dictionary<string, Question> QuestionBank { get; set; }

    // Flags
    public bool BankListReady { get; set; }
    public bool InitReady { get; set; }

    static void CreateDefault()
    {
        FirebaseController prefab = Resources.Load<FirebaseController>("Prefabs/PrefabFirebaseController");
        _instance = Instantiate(prefab);
        _instance.gameObject.name = "FirebaseController";
    }

    private void Awake()
    {
        // singleton
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        fb = FirebaseRest.Instance;
        Banks = new List<Bank>();
        BankListReady = false;
        InitReady = false;
    }

    public IEnumerator GetBankList()
    {
        Debug.Log("Getting the Bank List...");
        yield return fb.RetrieveBankList();
        Debug.Log("Got Bank List!");
        
        foreach (Bank bank in fb.BankList.Values)
        {
            Debug.Log(bank.name);
            Banks.Add(bank);
        }
        BankListReady = true;
    }

    /*public IEnumerator CreateQuestionBank(string bankname, List<Question> questions)
    {
        Debug.Log("Creating question bank...");
        yield return fb.CreateQuestionBank(bankname, questions);

        Debug.Log("Creation Completed.");
    }*/

    public IPromise<bool> CreateQuestionBank(string bankname, List<Question> questions)
    {
        var result = new Promise<bool>();
        fb.CreateQuestionBank(bankname, questions).Then(success =>
        {
            if (success)
                result.Resolve(true);
            else
                result.Resolve(false);
        });

        return result;
    }

    public IEnumerator InitQuizer(string bankname)
    {
        Debug.Log("Retrieving question bank: " + bankname);
        yield return fb.RetrieveQuestionBank(bankname);
        QuestionBank = fb.QuestionBank;

        Debug.Log("Retrieving History of player: " + Username);
        yield return fb.RetrieveHistory(Username, bankname);
        Dictionary<string, QuestionHistory>  HistoryDict = fb.QuestionHistory;

        Debug.Log("Construct History");
        // Initialize the question history if not yet encounter

        if (HistoryDict == null)
        {
            HistoryDict = new Dictionary<string, QuestionHistory>();
        }

        foreach (string id in QuestionBank.Keys)
        {
            if (!HistoryDict.ContainsKey(id))
            {
                HistoryDict.Add(id, new QuestionHistory(id, null, 0, false));
            }
        }

        Debug.Log("Sorting history record by Forgetting Algorithm");
        Histories = SortHistory(HistoryDict);

        // reduce the size if the number of questions reach the preferred maximun per day
        if (Histories.Count > Properties.Instance.MAX_QUESTIONS)
        {
            List<QuestionHistory> reducedHistires = new List<QuestionHistory>();

            int count = Properties.Instance.MAX_QUESTIONS;
            foreach (QuestionHistory history in Histories)
            {
                reducedHistires.Add(history);
                count--;
                if (count <= 0)
                {
                    break;
                }
            }

            Histories = reducedHistires;
        }

        InitReady = true;

        yield return 0;
    }

    // Sort the Question history from Dictionary to List and converting back
    /*public List<QuestionHistory> SortHistory(Dictionary<string, QuestionHistory> histories)
    {
        // covert to list first
        List<KeyValuePair<string, QuestionHistory>> temp = new List<KeyValuePair<string, QuestionHistory>>();
        foreach (string id in histories.Keys)
        {
            temp.Add(new KeyValuePair<string, QuestionHistory>(id, histories[id]));
        }

        // sort
        temp.Sort(delegate(KeyValuePair<string, QuestionHistory> pair1,
            KeyValuePair<string, QuestionHistory> pair2)
        {
            return CalculateForgettingRate(pair1.Value).CompareTo(CalculateForgettingRate(pair2.Value));
        });

        // convert back to dictionary
        foreach (KeyValuePair<string, QuestionHistory> item in temp)
        {
            Debug.Log(item.Value.ToString());
            Debug.Log(CalculateForgettingRate(item.Value));

        }

        return histories;
    }*/

    private List<QuestionHistory> SortHistory(Dictionary<string, QuestionHistory> histories)
    {
        // covert to list first
        List<QuestionHistory> sortList = new List<QuestionHistory>();
        List<QuestionHistory> result = new List<QuestionHistory>();

        foreach (string id in histories.Keys)
        {
            sortList.Add(histories[id]);
        }

        // sort
        sortList.Sort((q1, q2) =>
        {
            return CalculateForgettingRate(q1).CompareTo(CalculateForgettingRate(q2));
        });

        // convert back to dictionary



        foreach (QuestionHistory his in sortList)
        {
            double rate = CalculateForgettingRate(his);
            // Debug.Log(JsonConvert.SerializeObject(QuestionBank[his.Id], Formatting.Indented));
            // ColorPrint(rate);

            if (rate < Properties.Instance.MIN_RATE)
            {
                result.Add(his);
            }
        }

        return result;
    }

    // Forgetting algorithm
    private double CalculateForgettingRate(QuestionHistory history)
    {
        double result = 0.0;

        Question question = QuestionBank[history.Id];
        Debug.Log(JsonConvert.SerializeObject(question));

        if (history.MemoryStrength != 0)
        {
            DateTime dt = DateTime.Now;
            DateTime lastAttempt = DateTime.Parse(history.LastAttemptDate);
            int timeInterval = dt.AddDays(Properties.Instance.ADD_DAYS).Subtract(lastAttempt).Days;
            result = Math.Pow(Math.E, (-timeInterval / Convert.ToDouble(history.MemoryStrength)));
        }

        Debug.Log("<color=orange>Memory retention rate: " + result);

        return result;

        /*
         Question question = QuestionBank[id];
        Debug.Log(JsonConvert.SerializeObject(question));
        // <color=orange>" + log + "</color>"
        Debug.Log("<color=orange>Memory strength: " + CalculateForgettingRate(Histories[id]) + "</color>");
         */
    }

    public Question GetQuestion()
    {
        if (Histories.Count > 0)
        {
            return QuestionBank[Histories[0].Id];
        }
        else
        {
            ColorError("Histories.Count = 0, why you still call get question?");
            return null;
        }
    }

    // TODO
    public void RemoveQuestion(string id, string bankname)
    {
        // Find history by id
        QuestionHistory history = Histories.Find(his => his.Id == id);

        Debug.Log(JsonConvert.SerializeObject(history, Formatting.Indented));

        // Modify the memory strength and date
        history.LastAttemptDate = DateTime.Now.AddDays(Properties.Instance.ADD_DAYS).ToShortDateString().ToString();
        if (history.LastIsCorrect)
        {
            history.MemoryStrength += 2;
        }
        else
        {
            history.LastIsCorrect = true;
            history.MemoryStrength++;
        }

        // Update to db
        fb.UpdateHistory(Username, bankname, history); // TODO change to generic bankname

        // Remove from history list
        Histories.Remove(history);
    }

    public void ReorderQuestion(string id)
    {
        Debug.Log("Reorder question: " + id);
        // Find history by id
        QuestionHistory history = Histories.Find(his => his.Id == id);
        history.LastIsCorrect = false;

        Debug.Log("Reorder: Update history to false: " + JsonConvert.SerializeObject(history, Formatting.Indented));

        // Remove from history list
        Histories.Remove(history);

        Histories.Add(history);
    }

    public void ColorPrint(object log)
    {
        Debug.Log("<color=orange>" + log + "</color>");
    }

    public void ColorError(object log)
    {
        Debug.LogError("<color=red>" + log + "</color>");
    }
}
