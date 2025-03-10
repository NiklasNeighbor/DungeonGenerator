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

    private RectInt DebugRoom;
    private RectInt DebugRoom2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenRooms = new List<RectInt>();
        ClosedRooms = new List<RectInt>();

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
        while (true)
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
    }


}
