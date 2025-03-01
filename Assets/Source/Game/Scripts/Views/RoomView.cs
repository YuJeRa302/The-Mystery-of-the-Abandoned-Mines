using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public abstract class RoomView : MonoBehaviour
    {
        private readonly Vector3 _standartCameraArial = new Vector3(0, 1.5f, 1.6f);
        private readonly Vector3 _maxCameraArial = new Vector3(1f, 1.5f, 1.6f);

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
        [Header("Spawn Locations")]
        [SerializeField] private Transform[] _trapSpawnpoints;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [Header("Room Door Places")]
        [SerializeField] private RoomDoor[] _doors;
        [Header("Confiner Zone")]
        [SerializeField] private BoxCollider _confiner;
        [Header("NavMesh")]
        [SerializeField] private NavMeshSurface _navSurface;

        private RoomDoor _openDoor;
        private int _countEnemy;

        public event Action<RoomView> RoomEntering;

        public GameObject WallUpper => _wallUpper;
        public GameObject WallRight => _wallRight;
        public GameObject WallDown => _wallDown;
        public GameObject WallLeft => _wallLeft;
        public BoxCollider Confiner => _confiner;
        public NavMeshSurface NavSurface => _navSurface;
        public Transform[] EnemySpawnPoints => _enemySpawnPoints;
        public Transform[] TrapSpawnPoints => _trapSpawnpoints;
        public RoomData RoomData { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsComplete { get; private set; } = false;
        public int CountEnemy => _countEnemy;

        private void OnDestroy()
        {
            foreach (var door in _doors)
            {
                door.RoomDoorTaked -= OnDoorTaked;
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Player player))
                RoomEntering?.Invoke(this);
        }

        public void SetRoomStatus()//test
        {
            IsComplete = true;
        }

        public void SetCameraArial(bool canSeeDoor)
        {
            if (canSeeDoor)
                _confiner.size = _standartCameraArial;
            else
                _confiner.size = _maxCameraArial;
        }

        public void Initialize(RoomData roomData, int currentLevel, bool canSeeDoor)
        {
            SetCameraArial(canSeeDoor);

            RoomData = roomData;
            CurrentLevel = currentLevel;
            
            if(RoomData.Id == 1)
                _countEnemy = 1;
            else
                _countEnemy = UnityEngine.Random.Range(2, 6); //tests

            CreateDoorway();
            AddListener();
        }

        public void SetComplete()
        {
            IsComplete = true;
            UnlockRoom();
        }

        public void LockRoom()
        {
            if (IsComplete == true)
                return;

            foreach (var door in _doors)
            {
                door.Lock();
            }
        }

        public virtual void UnlockRoom()
        {
            foreach (var door in _doors)
            {
                door.Unlock();
            }

            if (_openDoor != null)
                _openDoor.SetDoorOpen();
        }

        private void AddListener() 
        {
            foreach (var door in _doors)
            {
                door.RoomDoorTaked += OnDoorTaked;
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

        private void OnDoorTaked(RoomDoor roomDoor) 
        {
            _openDoor = roomDoor;
        }
    }
}