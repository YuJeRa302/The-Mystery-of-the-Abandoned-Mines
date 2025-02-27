using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Room", menuName = "Create Room", order = 51)]
    public class RoomData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _roomName;
        [SerializeField] private RoomView _room;
        [SerializeField] private AnimationCurve _chanceFromDistance;
        [SerializeField] private EnemyData[] _enemyDatas;
        [SerializeField] private EnemyData[] _epicEnemyDatas;
        [SerializeField] private GameObject[] _traps;

        public int Id => _id;
        public string RoomName => _roomName;
        public RoomView Room => _room;
        public AnimationCurve ChanceFromDistance => _chanceFromDistance;
        public EnemyData[] EnemyData => _enemyDatas;
        public EnemyData[] EpicEnemyDatas => _epicEnemyDatas;
        public GameObject[] Traps => _traps;
    }
}