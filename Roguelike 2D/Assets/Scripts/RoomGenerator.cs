using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{

    [Header("Room Information")]
    public GameObject roomPrefab;
    public int roomNum;
    public Color startColor, endColor;

    [Header("Position control")]
    public Transform generatePoint;
    public float xOffset, yOffset;
    public LayerMask roomLayer;

    // stored the prefab of walls with different doors
    public WallType walls;

    // stored the generated rooms
    public List<Room> rooms = new List<Room>();

    // for finding final room
    List<GameObject> FarRoom = new List<GameObject>();
    List<GameObject> SecondFarRoom = new List<GameObject>();
    List<GameObject> SingleDoorRoom = new List<GameObject>();
    public GameObject FinalRoom;  // the final room of current level 

    [Header("Procedured Prefabs")]
    public GameObject[] Items;
    public GameObject[] Enemies;
    public GameObject Ladder;

    // UI
    [Header("UI")]
    public GameObject ProgressUIManager;
    public GameObject ImageCompleteMessage;

    public enum Direction
    {
        up, down, left, right
    };
    Direction direction;

    FirebaseController fc;

    bool completeQuiz;

    void Start()
    {
        StartCoroutine(Init());
    }

    private void Update()
    {
        /*if (completeQuiz)
        {
            SceneManager.LoadScene("Login");
        }*/
    }

    private IEnumerator Init()
    {
        completeQuiz = false;  // determine the quiz is done or not

        fc = FirebaseController.Instance;

        // yield return fc.GetBankList(); //TODO wait until we can select the question bank

        if (!fc.InitReady)
        {
            yield return fc.InitQuizer(Properties.Instance.BANK_NAME);   //TODO change by giving bank list, the property may store in game controller
        }

        // Update the number of question will be access in the UI
        ProgressUIManager.GetComponent<ProgressUIManager>().UpdateRemainingQuestions(fc.Histories.Count.ToString());

        int totalQuestions = FirebaseController.Instance.Histories.Count;
        if (totalQuestions >= 10)
        {
            GenerateRooms(10); // TODO

            // respawn enemies
            if (totalQuestions > 15)
            {
                RespawnEnemies(15);
            }
            else
            {
                RespawnEnemies(totalQuestions);
            }
        }
        else if (totalQuestions > 0)
        {
            int roomNumber = totalQuestions;
            if (roomNumber == 1)
            {
                GenerateRooms(2);
            }
            GenerateRooms(roomNumber);

            // respawn enemies
            RespawnEnemies(totalQuestions);
        }
        else
        {
            Debug.Log("No question need to be assessed.");

            //completeQuiz = true;
            StartCoroutine(CompleteQuiz());

        }
    }

    IEnumerator CompleteQuiz()
    {
        // Show complete message
        ImageCompleteMessage.SetActive(true);
        yield return new WaitForSeconds(5);

        // Exit game
        SceneManager.LoadScene("Login");
    }


    /*public void Init()
    {
        generatePoint.position.Set(0, 0, 0);    //reset room generating point
        rooms.Clear();
        FarRoom.Clear();
        SecondFarRoom.Clear();
        SingleDoorRoom.Clear();
        FinalRoom = null;
    }*/

    public void GenerateRooms(int roomNum)
    {
        // create random room by room number specified
        for (int i = 0; i < roomNum; i++)
        {
            rooms.Add(Instantiate(roomPrefab, generatePoint.position, Quaternion.identity).GetComponent<Room>());
            ChangePoint();
        }

        // Update room
        foreach (var room in rooms)
        {
            UpdateRoom(room, room.transform.position);
        }

        // Find final room to next level
        FindFinalRoom();


        // ladder to next level
        Instantiate(Ladder, FinalRoom.transform.position, Quaternion.identity);
    }

    // respawn enemies randomly
    public void RespawnEnemies(int total)
    {
        GameObject respawned = null;
        /*foreach (Room room in rooms)
        {
            int Count = 0;
            if (total > 1)
            {
                Count = Random.Range(0, 3);
            }
            else
            {
                Count = total;
            }
            
            for (int i = 0; i < Count; i++)
            {
                int randomPos = Random.Range(0, room.instantiablePosition.Count);
                Vector3 roomPos = room.instantiablePosition[randomPos];
                room.instantiablePosition.RemoveAt(randomPos);
                int rand = Random.Range(0, Enemies.Length);
                respawned = Instantiate(Enemies[rand], roomPos, Quaternion.identity);
                respawned.transform.SetParent(room.transform);
            }
        }*/

        while (total > 0)
        {
            // select a random room
            Room room = rooms[Random.Range(0, rooms.Count)];

            // select a random position in that room
            int randomPos = Random.Range(0, room.instantiablePosition.Count);
            Vector3 roomPos = room.instantiablePosition[randomPos];

            // select a random enemy to respawn
            int rand = Random.Range(0, Enemies.Length);
            respawned = Instantiate(Enemies[rand], roomPos, Quaternion.identity);
            respawned.transform.SetParent(room.transform);

            total--;
        }

    }

    // Change the genereating point randomly from top/bottom/left/right of current room
    public void ChangePoint()
    {
        direction = (Direction)Random.Range(0, 4);

        do
        {
            switch (direction)
            {
                case Direction.up:
                    generatePoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generatePoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generatePoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generatePoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
        } while (Physics2D.OverlapCircle(generatePoint.position, 0.2f, roomLayer));
    }

    // Update elements in each room
    // 1. Calculate the steps from starting room to each new room for finding farest room as final room
    // 2. Check any room is linked with the new room, then create the doors for linking room
    public void UpdateRoom(Room newRoom, Vector3 roomPosition)
    {
        // check the new room is linked with other generated room for setup door
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        // update the step from starting point to that new room, 
        // decide the final room when generation completed
        newRoom.UpdateRoomStep(xOffset, yOffset);

        GameObject roomWithDoorPrefab = null;

        // instantiate doors by the type of room
        switch (newRoom.doorTypeInBit)
        {
            case WallType.Door.left:
                roomWithDoorPrefab = Instantiate(walls.s_left, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.s_right, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top:
                roomWithDoorPrefab = Instantiate(walls.s_top, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.s_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.left:
                roomWithDoorPrefab = Instantiate(walls.d_top_left, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.d_top_right, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.d_top_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.left | WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.d_left_right, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.left | WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.d_left_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.right | WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.d_right_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.left | WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.t_top_left_right, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.left | WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.t_top_left_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.bottom | WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.t_top_right_Bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.left | WallType.Door.bottom | WallType.Door.right:
                roomWithDoorPrefab = Instantiate(walls.t_left_right_bottom, roomPosition, Quaternion.identity);
                break;
            case WallType.Door.top | WallType.Door.left | WallType.Door.right | WallType.Door.bottom:
                roomWithDoorPrefab = Instantiate(walls.q_all, roomPosition, Quaternion.identity);
                break;
        }

        // group the instantiated object
        roomWithDoorPrefab.transform.parent = newRoom.transform;
    }

    // find the one way room as the final room
    public void FindFinalRoom()
    {
        // find max step in generated rooms
        int maxStep = 0;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepFromStart > maxStep)
            {
                maxStep = rooms[i].stepFromStart;
            }
        }

        // find farest/second far rooms
        foreach (var room in rooms)
        {
            if (room.stepFromStart == maxStep)
            {
                FarRoom.Add(room.gameObject);
            }
            else if (room.stepFromStart == maxStep - 1)
            {
                SecondFarRoom.Add(room.gameObject);
            }
        }

        // find rooms with only one door
        for (int i = 0; i < FarRoom.Count; i++)
        {
            if (FarRoom[i].GetComponent<Room>().doorNum == 1)
            {
                SingleDoorRoom.Add(FarRoom[i]);
            }
        }

        for (int i = 0; i < SecondFarRoom.Count; i++)
        {
            if (SecondFarRoom[i].GetComponent<Room>().doorNum == 1)
            {
                SingleDoorRoom.Add(SecondFarRoom[i]);
            }
        }

        if (SingleDoorRoom.Count != 0)
        {
            FinalRoom = SingleDoorRoom[Random.Range(0, SingleDoorRoom.Count)];
        }
        else
        {
            FinalRoom = FarRoom[Random.Range(0, FarRoom.Count)];
        }

        FinalRoom.GetComponent<Room>().IsFinalRoom = true;
    }

}