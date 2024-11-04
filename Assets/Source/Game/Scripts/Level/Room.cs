using System;
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
        [Header("Room Door Places")]
        [SerializeField] private GameObject _doorUpper;
        [SerializeField] private GameObject _doorRight;
        [SerializeField] private GameObject _doorDown;
        [SerializeField] private GameObject _doorLeft;
        [Header("Spawn Locations")]
        [SerializeField] private Transform[] _trapSpawnpoints;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private Transform _bossSpawnPoint;

        public event Action<Room> RoomEntering;

        public GameObject WallUpper => _wallUpper;
        public GameObject WallRight => _wallRight;
        public GameObject WallDown => _wallDown;
        public GameObject WallLeft => _wallLeft;
        public Transform[] TrapSpawnPoints => _trapSpawnpoints;
        public Transform[] EnemySpawnPoints => _enemySpawnPoints;
        public Transform BossSpawnPoint => _bossSpawnPoint;
        public RoomData RoomData { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsComplete { get; private set; } = false;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Player player))
                if (IsComplete == false)
                    LockRoom();
        }

        public void Initialize(RoomData roomData, int currentLevel)
        {
            RoomData = roomData;
            CurrentLevel = currentLevel;
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

        private void LockRoom()
        {
            if (_wallLeft != null && _wallLeft.activeSelf == false)
                _doorLeft.SetActive(true);

            if (_wallRight != null && _wallRight.activeSelf == false)
                _doorRight.SetActive(true);

            if (_wallUpper != null && _wallUpper.activeSelf == false)
                _doorUpper.SetActive(true);

            if (_wallDown != null && _wallDown.activeSelf == false)
                _doorDown.SetActive(true);

            RoomEntering?.Invoke(this);
        }

        private void UnlockRoom()
        {
            if (_doorLeft != null && _doorLeft.activeSelf == true)
                _doorLeft.SetActive(false);

            if (_doorRight != null && _doorRight.activeSelf == true)
                _doorRight.SetActive(false);

            if (_doorUpper != null && _doorUpper.activeSelf == true)
                _doorUpper.SetActive(false);

            if (_doorDown != null && _doorDown.activeSelf == true)
                _doorDown.SetActive(false);
        }
    }
}