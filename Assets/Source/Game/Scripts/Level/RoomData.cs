using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Room", menuName = "Create Room", order = 51)]
    public class RoomData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _roomName;
        [SerializeField] private Room _room;
        [SerializeField] private AnimationCurve _chanceFromDistance;

        public int Id => _id;
        public string RoomName => _roomName;
        public Room Room => _room;
        public AnimationCurve ChanceFromDistance => _chanceFromDistance;
    }
}