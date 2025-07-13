using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Rooms
{
    public class RemovableWall : Wall
    {
        [SerializeField] private TypeRoomSide _typeRoomSide;

        public TypeRoomSide TypeRoomSide => _typeRoomSide;
    }
}