using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties : MonoBehaviour
{
    public int ADD_DAYS;
    public int MAX_QUESTIONS;
    [Range(0,1)]public float MIN_RATE;
    public string BANK_NAME;


    private static Properties _instance;
    public static Properties Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = FindObjectOfType<Properties>();
            if (_instance != null)
            {
                return _instance;
            }
            CreateDefault();
            return _instance;
        }
    }

    static void CreateDefault()
    {
        Properties prefab = Resources.Load<Properties>("Prefabs/PrefabProperties");
        _instance = Instantiate(prefab);
        _instance.gameObject.name = "Properties";
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
    }
}
