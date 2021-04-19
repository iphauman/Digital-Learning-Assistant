using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapHelper : MonoBehaviour
{
    GameObject sprite;
    private void OnEnable()
    {
        sprite = this.transform.parent.Find("Map_Rooms_1").gameObject;
        sprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sprite.SetActive(true);
        }
    }
}
