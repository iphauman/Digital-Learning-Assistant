using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float speed;

    public Transform follower;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (follower != null)
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(follower.position.x, follower.position.y, transform.position.z),
                speed * Time.deltaTime);
    }

    public void ChangeRoom(Transform nextRoom)
    {
        follower = nextRoom;
    }
}
