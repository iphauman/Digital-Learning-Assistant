using RSG;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class iDebug : MonoBehaviour
{
    public GameObject Field_BANK_NAME;
    public GameObject Field_ADD_DAYS;
    
    public void ChangeAddDays()
    {
        string str = Field_ADD_DAYS.GetComponent<Text>().text;
        if (!string.IsNullOrEmpty(str))
        {
            bool converted = int.TryParse(Field_ADD_DAYS.GetComponent<Text>().text, out int days);

            if (converted)
            {
                Properties.Instance.ADD_DAYS = days;
                Debug.Log("Changed the Properties.ADD_DAYS to " + Properties.Instance.ADD_DAYS);
            }
        }
    }

    public void ChangeBankName()
    {
        string str = Field_BANK_NAME.GetComponent<Text>().text;

        if (!string.IsNullOrEmpty(str))
        {
            Properties.Instance.BANK_NAME = str;
            Debug.Log("Changed the Properties.BANK_NAME to " + Properties.Instance.BANK_NAME);
        }
    }
}
