using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Dungeon/RoomShape")]
public class RoomShape : ScriptableObject
{
    public List<RoomShapeVariant> possibleRooms;
}
