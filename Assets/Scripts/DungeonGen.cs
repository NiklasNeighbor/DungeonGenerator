using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DungeonGen : MonoBehaviour
{

    private List<RectInt> OpenRooms;
    private List<RectInt> ClosedRooms;
    private RectInt DebugRoom;
    private RectInt DebugRoom2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenRooms = new List<RectInt>();
        ClosedRooms = new List<RectInt>();

        OpenRooms.Add(new RectInt(0, 0, 100, 50));
        StartCoroutine(SplitRoomsCoroutine());
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
        AlgorithmsUtils.DebugRectInt(DebugRoom, Color.blue);
        AlgorithmsUtils.DebugRectInt(DebugRoom2, Color.yellow);
    }

    public void DivideRect(RectInt room)
    {
        if (room.width > room.height)
        {
            RectInt newRoomA = new RectInt(room.x, room.y, GetPaddedRandom(room.width, 3), room.height);
            RectInt newRoomB = new RectInt(room.x + newRoomA.width, room.y + 0, room.width - newRoomA.width, room.height);
            OpenRooms.Add(newRoomA);
            OpenRooms.Add(newRoomB);
            //ClosedRooms.Add(room);
            OpenRooms.Remove(room);
            DebugRoom = newRoomB;
            DebugRoom2 = newRoomA;
        } else
        {
            RectInt newRoomA = new RectInt(room.x, room.y, room.width, GetPaddedRandom(room.height, 3));
            RectInt newRoomB = new RectInt(room.x + 0, room.y + newRoomA.height, room.width, room.height - newRoomA.height);
            OpenRooms.Add(newRoomA);
            OpenRooms.Add(newRoomB);
            //ClosedRooms.Add(room);
            OpenRooms.Remove(room);
            DebugRoom = newRoomB;
            DebugRoom2 = newRoomA;
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
}
