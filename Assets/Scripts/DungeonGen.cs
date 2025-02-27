using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{

    private List<RectInt> OpenRooms;
    private List<RectInt> ClosedRooms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenRooms = new List<RectInt>();
        ClosedRooms = new List<RectInt>();

        OpenRooms.Add(new RectInt(0,0,100,50));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (RectInt room in OpenRooms)
        {
            Gizmos.DrawWireCube(new Vector3(room.x, 0, room.y), new Vector3(room.width, 0, room.height));
        }
        
    }
}
