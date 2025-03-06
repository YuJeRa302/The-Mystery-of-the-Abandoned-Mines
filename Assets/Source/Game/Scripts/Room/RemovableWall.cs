using UnityEngine;

public class RemovableWall : Wall
{
    [SerializeField] private TypeRoomSide _typeRoomSide;

    public TypeRoomSide TypeRoomSide => _typeRoomSide;
}