using System.Collections.Generic;
using UnityEngine;

public class DungeonLocation
{
    
    public RectInt Room;
    public DungeonLocation SourceConnection;
    public List<DungeonLocation> NeighborLocations;
    public bool isDoor = true;

    public DungeonLocation(RectInt rectInt, bool _isDoor)
    {
        NeighborLocations = new List<DungeonLocation>();
        Room = rectInt;
        isDoor = _isDoor;
    }
}
