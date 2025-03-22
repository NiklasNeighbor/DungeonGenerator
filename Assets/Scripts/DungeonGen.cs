using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DungeonGen : MonoBehaviour
{
    public int MinRoomSize = 10;
    public int MaxRoomSize = 200;

    private List<DungeonLocation> OpenRooms;
    private List<DungeonLocation> ClosedRooms;

    private List<DungeonLocation> OpenWalls;
    private List<DungeonLocation> ClosedWalls;

    private List<DungeonLocation> Doors;

    private RectInt DebugRoom;
    private RectInt DebugRoom2;

    private bool DebugDoorBool = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenRooms = new List<DungeonLocation>();
        ClosedRooms = new List<DungeonLocation>();

        OpenWalls = new List<DungeonLocation>();
        ClosedWalls = new List<DungeonLocation>();

        Doors = new List<DungeonLocation>();

        OpenRooms.Add(new DungeonLocation(new RectInt(0, 0, 100, 50)));
        StartCoroutine(SplitRoomsCoroutine());
        StartCoroutine(WaitForRoomsCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (DungeonLocation room in OpenRooms)
        {
            AlgorithmsUtils.DebugRectInt(room.Room, Color.red);
        }
        foreach (DungeonLocation room in ClosedRooms)
        {
            AlgorithmsUtils.DebugRectInt(room.Room, Color.green);
        }

        if (OpenRooms.Count > 0)
        {
            AlgorithmsUtils.DebugRectInt(DebugRoom, Color.blue);
            AlgorithmsUtils.DebugRectInt(DebugRoom2, Color.yellow);
        }

        foreach (DungeonLocation Wall in ClosedWalls)
        {
            AlgorithmsUtils.DebugRectInt(Wall.Room, Color.cyan);
        }
        foreach (DungeonLocation door in Doors)
        {
            AlgorithmsUtils.DebugRectInt(door.Room, Color.yellow);
        }

        if (DebugDoorBool)
        {
            AlgorithmsUtils.DebugRectInt(DebugRoom, Color.blue);
            AlgorithmsUtils.DebugRectInt(DebugRoom2, Color.yellow);
        }
    }

    public void DivideRect(DungeonLocation room)
    {
        if (room.Room.width * room.Room.height > MaxRoomSize)
        {
            if (room.Room.width > room.Room.height)
            {
                RectInt newRoomA = new RectInt(room.Room.x, room.Room.y, GetPaddedRandom(room.Room.width, MinRoomSize) + 1, room.Room.height);
                RectInt newRoomB = new RectInt(room.Room.x + newRoomA.width -1, room.Room.y + 0, room.Room.width - newRoomA.width +1, room.Room.height);
                OpenRooms.Add(new DungeonLocation(newRoomA));
                OpenRooms.Add(new DungeonLocation(newRoomB));
                OpenRooms.Remove(room);
                DebugRoom = newRoomB;
                DebugRoom2 = newRoomA;
            }
            else
            {
                RectInt newRoomA = new RectInt(room.Room.x, room.Room.y, room.Room.width, GetPaddedRandom(room.Room.height, MinRoomSize) + 1);
                RectInt newRoomB = new RectInt(room.Room.x + 0, room.Room.y + newRoomA.height -1, room.Room.width, room.Room.height - newRoomA.height +1);
                OpenRooms.Add(new DungeonLocation(newRoomA));
                OpenRooms.Add(new DungeonLocation(newRoomB));
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

    public void FindSingleWall(DungeonLocation MainRoom)
    {
        foreach (DungeonLocation Room2 in OpenWalls)
        {
            if (MainRoom == Room2)
            {
                continue;
            }
            RectInt SharedWall = AlgorithmsUtils.Intersect(MainRoom.Room, Room2.Room);
            
            if (SharedWall.width > 2 || SharedWall.height > 2) //Ensure the wall in question is long enough to fit a door
            {
                DungeonLocation WallLocation = new DungeonLocation(SharedWall);
                WallLocation.SourceConnection = MainRoom;
                WallLocation.NeighborLocations.Add(Room2);
                ClosedWalls.Add(WallLocation);
                Debug.Log(SharedWall);
            }
        }
        OpenWalls.Remove(MainRoom);
    }

    public void WallsToDoors()
    {
        foreach(DungeonLocation wall in ClosedWalls)
        {
            if (wall.Room.width > wall.Room.height)
            {
                int DoorOffset = Random.Range(0 + 1, wall.Room.width - 2);
                RectInt NewDoor = new RectInt(wall.Room.x + DoorOffset, wall.Room.y, 1, 1);

                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor);
                wall.SourceConnection.NeighborLocations.Add(NewDoorLocation);
                NewDoorLocation.NeighborLocations.Add(wall.NeighborLocations[0]);

                Doors.Add(NewDoorLocation);
            }
            else
            {
                int DoorOffset = Random.Range(0 + 1, wall.Room.height - 2);
                RectInt NewDoor = new RectInt(wall.Room.x, wall.Room.y + DoorOffset, 1, 1);

                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor);
                wall.SourceConnection.NeighborLocations.Add(NewDoorLocation);
                NewDoorLocation.NeighborLocations.Add(wall.NeighborLocations[0]);

                Doors.Add(NewDoorLocation);
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
            DebugRoom = ClosedWalls[i].Room;
            Debug.Log(ClosedWalls[i]);

            if (ClosedWalls[i].Room.width > ClosedWalls[i].Room.height)
            {
                int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].Room.width - 2);
                RectInt NewDoor = new RectInt(ClosedWalls[i].Room.x + DoorLocation, ClosedWalls[i].Room.y, 1, 1);

                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor);
                ClosedWalls[i].SourceConnection.NeighborLocations.Add(NewDoorLocation);
                NewDoorLocation.NeighborLocations.Add(ClosedWalls[i].NeighborLocations[0]);

                DebugRoom2 = NewDoorLocation.Room;
                Doors.Add(NewDoorLocation);
            }
            else
            {
                int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].Room.height - 2);
                RectInt NewDoor = new RectInt(ClosedWalls[i].Room.x, ClosedWalls[i].Room.y + DoorLocation, 1, 1);
                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor);
                ClosedWalls[i].SourceConnection.NeighborLocations.Add(NewDoorLocation);
                NewDoorLocation.NeighborLocations.Add(ClosedWalls[i].NeighborLocations[0]);

                DebugRoom2 = NewDoorLocation.Room;
                Doors.Add(NewDoorLocation);
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Doors Done!");
    }

}
