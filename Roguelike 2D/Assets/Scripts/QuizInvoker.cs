using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizInvoker : MonoBehaviour
{
    public GameObject QuizUI;

    public GameObject BattleSign;

    public GameObject QuizManager;

    private void Start()
    {
        QuizManager = GameObject.Find("Quiz Manager");
        QuizUI = GameObject.Find("Game Manager").GetComponent<GameController>().GetQuizUI();
    }

    // show the battle signal to let player decide to fight against (take quiz from) the monster
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BattleSign.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BattleSign.SetActive(false);
        }
    }

    void Update()
    {
        // press space key to start quiz
        if (BattleSign.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            // shows the quiz ui
            QuizUI.SetActive(true);
            QuizManager.GetComponent<QuizManager>().UpdateContent(this.gameObject);
        }   
    }
}
