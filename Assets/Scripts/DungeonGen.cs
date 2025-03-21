using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DungeonGen : MonoBehaviour
{
    public int MinRoomSize = 10;
    public int MaxRoomSize = 200;

    private List<RectInt> OpenRooms;
    private List<RectInt> ClosedRooms;

    private List<RectInt> OpenWalls;
    private List<RectInt> ClosedWalls;

    private List<RectInt> Doors;

    private RectInt DebugRoom;
    private RectInt DebugRoom2;

    private bool DebugDoorBool = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenRooms = new List<RectInt>();
        ClosedRooms = new List<RectInt>();

        OpenWalls = new List<RectInt>();
        ClosedWalls = new List<RectInt>();

        Doors = new List<RectInt>();

        OpenRooms.Add(new RectInt(0, 0, 100, 50));
        StartCoroutine(SplitRoomsCoroutine());
        StartCoroutine(WaitForRoomsCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RectInt room in OpenRooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.red);
        }
        foreach (RectInt room in ClosedRooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.green);
        }

        if (OpenRooms.Count > 0)
        {
            AlgorithmsUtils.DebugRectInt(DebugRoom, Color.blue);
            AlgorithmsUtils.DebugRectInt(DebugRoom2, Color.yellow);
        }

        foreach (RectInt Wall in ClosedWalls)
        {
            AlgorithmsUtils.DebugRectInt(Wall, Color.cyan);
        }
        foreach (RectInt door in Doors)
        {
            AlgorithmsUtils.DebugRectInt(door, Color.yellow);
        }

        if (DebugDoorBool)
        {
            AlgorithmsUtils.DebugRectInt(DebugRoom, Color.blue);
            AlgorithmsUtils.DebugRectInt(DebugRoom2, Color.yellow);
        }
    }

    public void DivideRect(RectInt room)
    {
        if (room.width * room.height > MaxRoomSize)
        {
            if (room.width > room.height)
            {
                RectInt newRoomA = new RectInt(room.x, room.y, GetPaddedRandom(room.width, MinRoomSize) + 1, room.height);
                RectInt newRoomB = new RectInt(room.x + newRoomA.width -1, room.y + 0, room.width - newRoomA.width +1, room.height);
                OpenRooms.Add(newRoomA);
                OpenRooms.Add(newRoomB);
                OpenRooms.Remove(room);
                DebugRoom = newRoomB;
                DebugRoom2 = newRoomA;
            }
            else
            {
                RectInt newRoomA = new RectInt(room.x, room.y, room.width, GetPaddedRandom(room.height, MinRoomSize) + 1);
                RectInt newRoomB = new RectInt(room.x + 0, room.y + newRoomA.height -1, room.width, room.height - newRoomA.height +1);
                OpenRooms.Add(newRoomA);
                OpenRooms.Add(newRoomB);
                OpenRooms.Remove(room);
                DebugRoom = newRoomB;
                DebugRoom2 = newRoomA;
            }
        } else
        {
            ClosedRooms.Add(room);
            OpenRooms.Remove(room);

        }
    }

    public int GetPaddedRandom(int MaxNumber, int Padding)
    {
        return Random.Range(0 + Padding, MaxNumber - Padding);
    }


    public IEnumerator SplitRoomsCoroutine()
    {
        while (OpenRooms.Count > 0)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
           // foreach (RectInt room in OpenRooms)
            {
                DivideRect(OpenRooms[0]);
            }
            yield return new WaitForEndOfFrame();

        }
    }

    public IEnumerator WaitForRoomsCoroutine()
    {
        yield return new WaitUntil(() => OpenRooms.Count < 1);
        Debug.Log("Rooms Done!");
        FindWalls();
    }



    public void FindWalls()
    {
        for(int i = 0; i < ClosedRooms.Count; i++)
        {
            OpenWalls.Add(ClosedRooms[i]);
        }
        StartCoroutine(FindWallCoroutine());

        /*
        while (OpenWalls[0] != null)
        {
            foreach (RectInt Room2 in OpenWalls)
            {
                if (OpenWalls[0] == Room2)
                {
                    continue;
                }
                RectInt SharedWall = AlgorithmsUtils.Intersect(OpenWalls[0], Room2);
                Debug.Log(SharedWall);
            }
            OpenWalls.Remove(OpenWalls[0]);
        } 
        */
    }

    public IEnumerator FindWallCoroutine()
    {
        while (OpenWalls.Count > 0)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            // foreach (RectInt room in OpenRooms)
            {
                FindSingleWall(OpenWalls[0]);
            }
            yield return new WaitForEndOfFrame();

        }
        Debug.Log("Walls Done!");
        StartCoroutine(WallToDoorCoroutine());
        //WallsToDoors();
    }

    public void FindSingleWall(RectInt MainRoom)
    {
        foreach (RectInt Room2 in OpenWalls)
        {
            if (MainRoom == Room2)
            {
                continue;
            }
            RectInt SharedWall = AlgorithmsUtils.Intersect(MainRoom, Room2);
            
            if (SharedWall.width > 2 || SharedWall.height > 2) //Ensure the wall in question is long enough to fit a door
            {
                ClosedWalls.Add(SharedWall);
                Debug.Log(SharedWall);
            }
        }
        OpenWalls.Remove(MainRoom);
    }

    public void WallsToDoors()
    {
        foreach(RectInt wall in ClosedWalls)
        {
            if (wall.width > wall.height)
            {
                int DoorLocation = Random.Range(0 + 1, wall.width - 2);
                RectInt NewDoor = new RectInt(wall.x + DoorLocation, wall.y, 1, 1);
                Doors.Add(NewDoor);
            }
            else
            {
                int DoorLocation = Random.Range(0 + 1, wall.height - 2);
                RectInt NewDoor = new RectInt(wall.x, wall.y + DoorLocation, 1, 1);
                Doors.Add(NewDoor);
            }
        }
    }
    //TODO: figure out why the doors spawn weird at times.

    public IEnumerator WallToDoorCoroutine()
    {
        
        for (int i = 0; i < ClosedWalls.Count; i++)
        {
            
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            DebugDoorBool = true;
            DebugRoom = ClosedWalls[i];
            Debug.Log(ClosedWalls[i]);

            if (ClosedWalls[i].width > ClosedWalls[i].height)
            {
                int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].width - 2);
                RectInt NewDoor = new RectInt(ClosedWalls[i].x + DoorLocation, ClosedWalls[i].y, 1, 1);
                DebugRoom2 = NewDoor;
                Doors.Add(NewDoor);
            }
            else
            {
                int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].height - 2);
                RectInt NewDoor = new RectInt(ClosedWalls[i].x, ClosedWalls[i].y + DoorLocation, 1, 1);
                DebugRoom2 = NewDoor;
                Doors.Add(NewDoor);
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Doors Done!");
    }

}
