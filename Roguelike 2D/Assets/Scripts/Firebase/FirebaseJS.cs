using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;

public class FirebaseJS : MonoBehaviour
{
    public Text text;

    [DllImport("__Internal")]
    public static extern void GetJSON(string path, string objectName, string callback, string fallback);

    private void Start() => GetJSON("banks/list", gameObject.name, "OnRequestSuccess", "OnRequestFailure");

    // debug
    private void OnRequestSuccess(string data)
    {
        //Debug.Log("OnRequestSuccess: " + data);
        text.text = "OnRequestSuccess: " + data;
    }

    private void OnRequestFailure(string error)
    {
        //Debug.LogError(error);
        text.text = error;
    }
}
