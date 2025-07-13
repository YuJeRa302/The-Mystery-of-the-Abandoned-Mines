using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Rooms
{
    public class DoorWayView : MonoBehaviour
    {
        [SerializeField] private RoomDoorView _roomDoor;
        [SerializeField] private TypeRoomSide _typeRoomSide;

        public TypeRoomSide TypeRoomSide => _typeRoomSide;
        public RoomDoorView RoomDoor => _roomDoor;
    }
}