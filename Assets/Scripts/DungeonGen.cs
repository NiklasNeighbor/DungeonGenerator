using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DungeonGen : MonoBehaviour
{
    public int MinRoomSize = 10;
    public int MaxRoomSize = 200;
    public enum GenerationStyle {OnSpace, Timed, Instant}
    public GenerationStyle generate = GenerationStyle.OnSpace;
    public float generationSpeed = 0.5f;

    private List<DungeonLocation> OpenRooms;
    private List<DungeonLocation> ClosedRooms;

    private List<DungeonLocation> OpenWalls;
    private List<DungeonLocation> ClosedWalls;

    private List<DungeonLocation> OpenNodes;
    private List<DungeonLocation> ClosedNodes;

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

        OpenNodes = new List<DungeonLocation>();
        ClosedNodes = new List<DungeonLocation>();

        Doors = new List<DungeonLocation>();

        OpenRooms.Add(new DungeonLocation(new RectInt(0, 0, 100, 50), false));
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

    public IEnumerator GraphDebugDraw(DungeonLocation StartNode)
    {
        foreach(DungeonLocation room in ClosedRooms)
        {
            OpenNodes.Add(room);
        }
        foreach(DungeonLocation door in Doors)
        {
            OpenNodes.Add(door);
        }

        while (OpenNodes.Count > 0)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return new WaitForEndOfFrame();

            if (OpenNodes[0].isDoor)
            {
                foreach (DungeonLocation room in ClosedRooms)
                {
                    if (AlgorithmsUtils.Intersects(OpenNodes[0].Room, room.Room) && OpenNodes.Contains(room))
                    {
                        //DebugExtension.DrawCircle(room.Room.center, Vector3.up, Color.yellow);
                        //Debug.DrawLine(OpenNodes[0].Room.center, room.Room.center);
                        OpenNodes[0].NeighborLocations.Add(room);
                    }
                }
            } else
            {
                foreach (DungeonLocation door in Doors)
                {
                    if (AlgorithmsUtils.Intersects(OpenNodes[0].Room, door.Room) && OpenNodes.Contains(door))
                    {
                        //DebugExtension.DrawCircle(door.Room.center, Vector3.up, Color.yellow);
                        //Debug.DrawLine(OpenNodes[0].Room.center, door.Room.center);
                        OpenNodes[0].NeighborLocations.Add(door);
                    }
                }
            }

            ClosedNodes.Add(OpenNodes[0]);
            OpenNodes.Remove(OpenNodes[0]);
        }



        if (ClosedNodes.Count == ClosedRooms.Count + Doors.Count)
        {
            Debug.Log("All Rooms Are Connected!");
            Debug.Log("There are " + ClosedNodes.Count + " ClosedNodes and " + (ClosedRooms.Count + Doors.Count) + "ClosedRooms and Doors!");
        } else if (ClosedNodes.Count < ClosedRooms.Count + Doors.Count)
        {
            Debug.LogWarning("Not all Rooms are connected!");
            Debug.LogWarning("There are " + ClosedNodes.Count + " ClosedNodes and " + (ClosedRooms.Count + Doors.Count) + "ClosedRooms and Doors!");
        }
        else
        {
            Debug.LogWarning("What?");
            Debug.LogWarning("There are " + ClosedNodes.Count + " ClosedNodes but only " + (ClosedRooms.Count + Doors.Count) + "ClosedRooms and Doors!");
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (DungeonLocation node in OpenNodes)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(ToVector3(node.Room.center), 1f);
            }

            foreach (DungeonLocation node in ClosedNodes)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(ToVector3(node.Room.center), 1f);
                foreach (DungeonLocation neighbor in node.NeighborLocations)
                {
                    Gizmos.DrawLine(ToVector3(node.Room.center), ToVector3(neighbor.Room.center));
                }
            }
        }
    }

    public Vector3 ToVector3(Vector2 Coordinates)
    {
        return new Vector3(Coordinates.x, 0, Coordinates.y);
    }

    public void DivideRect(DungeonLocation room)
    {
        if (room.Room.width * room.Room.height > MaxRoomSize)
        {
            if (room.Room.width > room.Room.height)
            {
                RectInt newRoomA = new RectInt(room.Room.x, room.Room.y, GetPaddedRandom(room.Room.width, MinRoomSize) + 1, room.Room.height);
                RectInt newRoomB = new RectInt(room.Room.x + newRoomA.width -1, room.Room.y + 0, room.Room.width - newRoomA.width +1, room.Room.height);
                OpenRooms.Add(new DungeonLocation(newRoomA,false));
                OpenRooms.Add(new DungeonLocation(newRoomB, false));
                OpenRooms.Remove(room);
                DebugRoom = newRoomB;
                DebugRoom2 = newRoomA;
            }
            else
            {
                RectInt newRoomA = new RectInt(room.Room.x, room.Room.y, room.Room.width, GetPaddedRandom(room.Room.height, MinRoomSize) + 1);
                RectInt newRoomB = new RectInt(room.Room.x + 0, room.Room.y + newRoomA.height -1, room.Room.width, room.Room.height - newRoomA.height +1);
                OpenRooms.Add(new DungeonLocation(newRoomA, false));
                OpenRooms.Add(new DungeonLocation(newRoomB, false));
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

            switch (generate)
            {
                case GenerationStyle.OnSpace:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    break;
                case GenerationStyle.Timed:
                    yield return new WaitForSeconds(generationSpeed); 
                    break;
            }
            
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
            switch (generate)
            {
                case GenerationStyle.OnSpace:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    break;
                case GenerationStyle.Timed:
                    yield return new WaitForSeconds(generationSpeed);
                    break;
            }
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
                DungeonLocation WallLocation = new DungeonLocation(SharedWall, false);
                //WallLocation.SourceConnection = MainRoom;
                //WallLocation.NeighborLocations.Add(Room2);
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

                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor, true);
                //wall.SourceConnection.NeighborLocations.Add(NewDoorLocation);
                //NewDoorLocation.NeighborLocations.Add(wall.NeighborLocations[0]);

                Doors.Add(NewDoorLocation);
            }
            else
            {
                int DoorOffset = Random.Range(0 + 1, wall.Room.height - 2);
                RectInt NewDoor = new RectInt(wall.Room.x, wall.Room.y + DoorOffset, 1, 1);

                DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor, true);
                //wall.SourceConnection.NeighborLocations.Add(NewDoorLocation);
                //NewDoorLocation.NeighborLocations.Add(wall.NeighborLocations[0]);

                Doors.Add(NewDoorLocation);
            }
        }
    }
    

    public IEnumerator WallToDoorCoroutine()
    {
        
        for (int i = 0; i < ClosedWalls.Count; i++)
        {

            switch (generate)
            {
                case GenerationStyle.OnSpace:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    break;
                case GenerationStyle.Timed:
                    yield return new WaitForSeconds(generationSpeed);
                    break;
            }

            DebugDoorBool = true;
            DebugRoom = ClosedWalls[i].Room;
            Debug.Log(ClosedWalls[i]);

            if (ClosedWalls[i].Room.width > ClosedWalls[i].Room.height)
            {
                SplitHorizontally(i);
            }
            else
            {
                SplitVertically(i);
            }
            yield return new WaitForEndOfFrame();
        }
        DebugDoorBool = false;
        Debug.Log("Doors Done!");
        StartCoroutine(GraphDebugDraw(ClosedRooms[0]));
    }

    private void SplitVertically(int i)
    {
        int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].Room.height - 2);
        RectInt NewDoor = new RectInt(ClosedWalls[i].Room.x, ClosedWalls[i].Room.y + DoorLocation, 1, 1);
        DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor, true);
        //ClosedWalls[i].SourceConnection.NeighborLocations.Add(NewDoorLocation);
        //NewDoorLocation.NeighborLocations.Add(ClosedWalls[i].NeighborLocations[0]);

        DebugRoom2 = NewDoorLocation.Room;
        Doors.Add(NewDoorLocation);
    }

    private void SplitHorizontally(int i)
    {
        int DoorLocation = Random.Range(0 + 1, ClosedWalls[i].Room.width - 2);
        RectInt NewDoor = new RectInt(ClosedWalls[i].Room.x + DoorLocation, ClosedWalls[i].Room.y, 1, 1);

        DungeonLocation NewDoorLocation = new DungeonLocation(NewDoor, true);
        //ClosedWalls[i].SourceConnection.NeighborLocations.Add(NewDoorLocation);
        //NewDoorLocation.NeighborLocations.Add(ClosedWalls[i].NeighborLocations[0]);

        DebugRoom2 = NewDoorLocation.Room;
        Doors.Add(NewDoorLocation);
    }
}
