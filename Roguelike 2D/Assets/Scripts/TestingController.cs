using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingController : MonoBehaviour
{
    public GameObject TestingAddDays;

    public void AddDays()
    {
        string days = TestingAddDays.gameObject.GetComponentInChildren<Text>().text;
        Debug.Log("Changing Properties ADD_DAYS to " + days);

        if (!string.IsNullOrEmpty(days))
        {
            int daysInInt = Properties.Instance.ADD_DAYS;
            Debug.Log("Trying to parse string to int");
            if (int.TryParse(days, out daysInInt))
            {
                Properties.Instance.ADD_DAYS = daysInInt;
            }
        }

        Debug.Log("ADD_DAYS after change: " + Properties.Instance.ADD_DAYS);
    }
}
