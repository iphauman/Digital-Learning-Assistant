using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;
    // Types of different object for procedured generation
    [Header("Procedured Prefabs")]
    public GameObject[] Items;
    public GameObject[] Enemies;
    public GameObject Ladder;

    // Room generator
    public GameObject RoomGenerator;
    private RoomGenerator rg;

    private List<GameObject> RespanwnPosition;

    // Questions
    FirebaseController fc;

    void Start()
    {
        /*fc = FirebaseController.Instance;
        rg = RoomGenerator.GetComponent<RoomGenerator>();
        NewLevel();*/
        
    }

    /*public void NewLevel()
    {
        // initialize game level
        rg.Init();
        // rg.GenerateRooms();

        // respawn enemies
        RespawnEnemies();
        
        // ladder to next level
        Instantiate(Ladder, rg.FinalRoom.transform.position, Quaternion.identity);
    }*/

    // respawn enemies randomly
    public void RespawnEnemies()
    {
        GameObject respawned = null;
        foreach (Room room in rg.rooms)
        {
            int Count = Random.Range(0,3);
            for (int i = 0; i < Count; i++)
            {
                int randomPos = Random.Range(0, room.instantiablePosition.Count);
                Vector3 roomPos = room.instantiablePosition[randomPos];
                room.instantiablePosition.RemoveAt(randomPos);
                int rand = Random.Range(0, Enemies.Length);
                respawned = Instantiate(Enemies[rand], roomPos, Quaternion.identity);
                respawned.transform.SetParent(room.transform);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }*/
    }

    [System.Serializable]
    public class Range
    {
        public int min;
        public int max;

        public Range(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
