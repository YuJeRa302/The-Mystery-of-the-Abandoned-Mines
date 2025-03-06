using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class DoorWayView : MonoBehaviour
    {
        [SerializeField] private RoomDoorView _roomDoor;
        [SerializeField] private TypeRoomSide _typeRoomSide;

        public TypeRoomSide TypeRoomSide => _typeRoomSide;
        public RoomDoorView RoomDoor => _roomDoor;
    }
}