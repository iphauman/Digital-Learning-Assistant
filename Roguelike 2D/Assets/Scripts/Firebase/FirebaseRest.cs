using FullSerializer;
using Newtonsoft.Json;
using Proyecto26;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using RSG;

public class FirebaseRest
{
    static readonly FirebaseRest _instance = new FirebaseRest();

    public static FirebaseRest Instance
    {
        get
        {
            return _instance;
        }
    }

    private readonly string endpoint = "";

    // flag
    //public bool GotList { get; set; }
    //public bool GotBank { get; set; }
    //public bool GotHistory { get; set; }

    public Dictionary<string, Bank> BankList;

    public Dictionary<string, Question> QuestionBank;

    public Dictionary<string, QuestionHistory> QuestionHistory;

    /*public IEnumerator CreateQuestionBank(string bankName, List<Question> questions)
    {
        QuestionHistory = new Dictionary<string, QuestionHistory>();
        // Add the name of question bank to collection as index
        CreateBank(bankName); // TODO wait until the select question bank completed

        // Add questions to specified question bank
        int count = 0;
        bool isCompleted = false;

        foreach (Question question in questions)
        {
            string path = endpoint + "QuestionBank/" + bankName + "/" + question.id + "/.json";
            RestClient.Put(path, question)
            .Then(res =>
            {
                count++;
                if (count == questions.Count)
                {
                    isCompleted = true;
                    Debug.Log("Uploaded question bank.");
                }
            })
            .Catch(err =>
            {
                Debug.Log("Error: " + err.Message);
            });
        }

        yield return new WaitUntil(() => isCompleted);
    }*/

    public IPromise<bool> CreateQuestionBank(string bankname, List<Question> questions)
    {
        var success = new Promise<bool>();

        foreach (Question question in questions)
        {
            string path = endpoint + "QuestionBank/" + bankname + "/" + question.id + "/.json";
            RestClient.Put(path, question)
            .Then(res =>
            {
                Debug.Log("Uploaded question bank.");
                success.Resolve(true);
            })
            .Catch(err =>
            {
                // Debug.Log("Error: " + err.Message);
                success.Reject(err);
            });
        }

        return success;
    }

    public void CreateBank(string bankname)
    {
        Bank bank = new Bank { name = bankname };
        string bankCollectionPath = endpoint + "BankList/" + GenerateUUID() + "/.json";
        RestClient.Put(bankCollectionPath, bank)
            .Then(res => Debug.Log("Success:" + JsonUtility.ToJson(res, true)))
            .Catch(err => Debug.Log("Error: " + err.Message));
    }

    public void AddQuestionToBank(string bankName, Question question)
    {
        string path = endpoint + "QuestionBank/" + bankName + "/" + question.id + "/.json";
        RestClient.Put(path, question)
        .Then(res => Debug.Log("Success:" + JsonUtility.ToJson(res, true)))
        .Catch(err => Debug.Log("Error: " + err.Message));
    }

    public IEnumerator RetrieveBankList()
    {
        BankList = new Dictionary<string, Bank>();
        bool GotList = false;

        // get data from database
        RestClient.Get(endpoint + "BankList/.json").Then(response =>
        {
            // parse data
            fsSerializer serializer = new fsSerializer();
            fsData data = fsJsonParser.Parse(response.Text);
            serializer.TryDeserialize(data, ref BankList);

            foreach (var question in BankList.Values)
            {
                Debug.Log(JsonConvert.SerializeObject(question, Formatting.Indented));
            }
            GotList = true;
        }).Catch(err =>
        {
            Debug.LogError("Error: " + err.Message);
            QuitGame();
        });
        yield return new WaitUntil(() => GotList);
    }

    public IEnumerator RetrieveQuestionBank(string bankName)
    {
        QuestionBank = new Dictionary<string, Question>();
        bool Retrieved = false;

        // get data from database
        RestClient.Get(endpoint + "QuestionBank/" + bankName + "/.json").Then(response =>
        {
            // parse data
            fsSerializer serializer = new fsSerializer();
            fsData data = fsJsonParser.Parse(response.Text);
            serializer.TryDeserialize(data, ref QuestionBank);

            foreach (var question in QuestionBank.Values)
            {
                // Debug.Log(JsonConvert.SerializeObject(question, Formatting.Indented));
            }
            Retrieved = true;

            Debug.Log("Retrieved question bank: " + bankName);


        }).Catch(err =>
        {
            Debug.LogError("Error: " + err.Message);
            QuitGame();
        });
        yield return new WaitUntil(() => Retrieved);
    }

    public void UpdateHistory(string username, string bankname, QuestionHistory history)
    {
        string path = endpoint + "History/" + username + "/" + bankname + "/" + history.Id;

        RestClient.Put(path + "/.json", history).Then(response =>
        {
            Debug.Log("Updated the history record: " + history.Id);
        }).Catch(err =>
        {
            Debug.Log("Error: " + err.Message);
        });
        Debug.Log("Updated History");
    }

    public IEnumerator RetrieveHistory(string username, string bankname)
    {
        bool GotHistory = false;
        QuestionHistory = new Dictionary<string, QuestionHistory>();

        RestClient.Get(endpoint + "History/" + username + "/" + bankname + "/.json").Then(response =>
        {
            // parse data
            fsSerializer serializer = new fsSerializer();
            fsData data = fsJsonParser.Parse(response.Text);
            serializer.TryDeserialize(data, ref QuestionHistory);

            // debug data
            foreach (var history in QuestionHistory.Values)
            {
                // Debug.Log(JsonConvert.SerializeObject(history, Formatting.Indented));
            }

            if (QuestionHistory.Count == 0)
            {
                Debug.Log("No history");
            }

            GotHistory = true;
        }).Catch(err =>
        {
            Debug.Log("Error: " + err.Message);
            GotHistory = true;
        });

        yield return new WaitUntil(() => GotHistory);
    }

    // For exit the game in Unity editor
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public string GenerateUUID()
    {
        return System.Guid.NewGuid().ToString();
    }
}
