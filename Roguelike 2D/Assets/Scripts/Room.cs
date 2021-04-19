using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameObject doorLeft, doorRight, doorUp, doorDown;    // game object of doors
    public bool roomLeft, roomRight, roomUp, roomDown;  // bool to determine the doors placed in the room
    private GameObject wallSprite;  // for storing the sprite of the wall for differnt kind of rooms
    public Text text;   // debug

    // determine the types of door of room
    // e.g. how the doors is placed in the room
    public int doorNum;
    public WallType.Door doorTypeInBit = 0;

    // final room
    public int stepFromStart;   // room steps from starting point
    private bool isFinalRoom;   // bool of final room
    public bool IsFinalRoom { get { return isFinalRoom; } set { isFinalRoom = true; } }

    // possible position for respawn object to avoid overlapping
    public List<Vector3> instantiablePosition;
    public int roomWidthInHalf;
    public int roomHeightInHalf;


    void Start()
    {
        // set defalut value of doors
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);


        
    }

    // update the steps from starting point, greatest value will determine the final room
    public void UpdateRoomStep(float xOffset, float yOffset)
    {
        if (roomUp)
        {
            doorNum++;
            doorTypeInBit |= WallType.Door.top;
        }

        if (roomDown)
        {
            doorNum++;
            doorTypeInBit |= WallType.Door.bottom;
        }

        if (roomLeft)
        {
            doorNum++;
            doorTypeInBit |= WallType.Door.left;
        }

        if (roomRight)
        {
            doorNum++;
            doorTypeInBit |= WallType.Door.right;
        }

        stepFromStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));
        text.text = stepFromStart.ToString() + "," + doorTypeInBit.ToString();


        int rowStart = ((int)(transform.position.y) - roomHeightInHalf);
        int rowEnd = ((int)(transform.position.y) + roomHeightInHalf);

        int colStart = ((int)(transform.position.x) - roomWidthInHalf);
        int colEnd = ((int)(transform.position.x) + roomWidthInHalf);

        for (int x = colStart; x <= colEnd; x++)
        {
            for (int y = rowStart; y <= rowEnd; y++)
            {
                instantiablePosition.Add(new Vector3(x, y, 0));
            }
        }

    }

    // change the camera position to follow player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraController.instance.ChangeRoom(transform);
        }
    }
}
