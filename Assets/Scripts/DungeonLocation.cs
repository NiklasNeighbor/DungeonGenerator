using System.Collections.Generic;
using UnityEngine;

public class DungeonLocation
{
    
    public RectInt Room;
    public DungeonLocation SourceConnection;
    public List<DungeonLocation> NeighborLocations;
    public bool isAlone = true;

    public DungeonLocation(RectInt rectInt)
    {
        NeighborLocations = new List<DungeonLocation>();
        Room = rectInt;
    }
}
