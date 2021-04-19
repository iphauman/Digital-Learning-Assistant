using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private GameObject mapPanel;

    private void Start()
    {
        mapPanel = this.transform.Find("Map Panel").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(mapPanel.activeSelf);
            mapPanel.SetActive(!mapPanel.activeSelf);
        }
    }
}
