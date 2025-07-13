using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Views;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Rooms
{
    [CreateAssetMenu(fileName = "New Room", menuName = "Create Room", order = 51)]
    public class RoomData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _roomName;
        [SerializeField] private Color[] _tierColor;
        [SerializeField] private RoomView _room;
        [SerializeField] private AnimationCurve _chanceFromDistance;
        [SerializeField] private EnemyData[] _enemyDatas;
        [SerializeField] private EnemyData[] _epicEnemyDatas;
        [SerializeField] private GameObject[] _traps;
        [SerializeField] private string _roomDescription;

        public int Id => _id;
        public Color[] TierColor => _tierColor;
        public string RoomName => _roomName;
        public RoomView Room => _room;
        public AnimationCurve ChanceFromDistance => _chanceFromDistance;
        public EnemyData[] EnemyDatas => _enemyDatas;
        public GameObject[] Traps => _traps;
        public string RoomDescription => _roomDescription;
    }
}