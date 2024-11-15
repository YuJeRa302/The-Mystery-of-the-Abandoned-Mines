using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Room : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly int _rotateCount = 4;
        private readonly float _degreeRotation = 90.0f;

        [Header("Room Wall Places")]
        [SerializeField] private GameObject _wallUpper;
        [SerializeField] private GameObject _wallRight;
        [SerializeField] private GameObject _wallDown;
        [SerializeField] private GameObject _wallLeft;
        [Header("Room Doorway Places")]
        [SerializeField] private GameObject _doorwayUpper;
        [SerializeField] private GameObject _doorwayRight;
        [SerializeField] private GameObject _doorwayDown;
        [SerializeField] private GameObject _doorwayLeft;
        [Header("Room Door Places")]
        [SerializeField] private RoomDoor[] _doors;
        [Header("Spawn Locations")]
        [SerializeField] private Transform[] _trapSpawnpoints;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private Transform _bossSpawnPoint;
        [Header("Confiner Zone")]
        [SerializeField] private BoxCollider _confiner;
        [Header("NavMesh")]
        [SerializeField] private NavMeshSurface _navSurface;

        public event Action<Room> RoomEntering;

        public GameObject WallUpper => _wallUpper;
        public GameObject WallRight => _wallRight;
        public GameObject WallDown => _wallDown;
        public GameObject WallLeft => _wallLeft;
        public Transform[] TrapSpawnPoints => _trapSpawnpoints;
        public Transform[] EnemySpawnPoints => _enemySpawnPoints;
        public Transform BossSpawnPoint => _bossSpawnPoint;
        public BoxCollider Confiner => _confiner;
        public NavMeshSurface NavSurface => _navSurface;
        public RoomData RoomData { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsComplete { get; private set; } = false;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Player player))
                if (IsComplete == false)
                    LockRoom();
                else
                    RoomEntering?.Invoke(this);
        }

        public void SetRoomStatus()//test
        {
            IsComplete = true;
        }

        public void Initialize(RoomData roomData, int currentLevel)
        {
            RoomData = roomData;
            CurrentLevel = currentLevel;
            CreateDoorway();
        }

        public void SetComplete()
        {
            IsComplete = true;
            UnlockRoom();
        }

        public void RotateWallRandomly()
        {
            int count = _rnd.Next(_rotateCount);

            for (int index = 0; index < count; index++)
            {
                transform.Rotate(0, _degreeRotation, 0);

                GameObject temp = _wallLeft;
                _wallLeft = _wallDown;
                _wallDown = _wallRight;
                _wallRight = _wallUpper;
                _wallUpper = temp;
            }
        }

        private void CreateDoorway() 
        {
            if (_wallLeft != null && _wallLeft.activeSelf == false)
                _doorwayLeft.SetActive(true);

            if (_wallRight != null && _wallRight.activeSelf == false)
                _doorwayRight.SetActive(true);

            if (_wallUpper != null && _wallUpper.activeSelf == false)
                _doorwayUpper.SetActive(true);

            if (_wallDown != null && _wallDown.activeSelf == false)
                _doorwayDown.SetActive(true);
        }

        private void LockRoom()
        {
            if (IsComplete == true)
                return;

            foreach (var door in _doors) 
            {
                door.Lock();
            }

            RoomEntering?.Invoke(this);
        }

        private void UnlockRoom()
        {
            foreach (var door in _doors)
            {
                door.Unlock();
            }
        }
    }
}