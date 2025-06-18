using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public abstract class RoomView : MonoBehaviour
    {
        private readonly Vector3 _standartCameraArial = new Vector3(0f, 0f, 1.78f);
        private readonly Vector3 _maxCameraArial = new Vector3(1f, 0f, 1.78f);

        [Header("Room Lights")]
        [SerializeField] private List<Light> _lights;
        [Header("Room Wall")]
        [SerializeField] private List<Wall> _walls;
        [Header("Removable Wall Places")]
        [SerializeField] private List<RemovableWall> _removableWalls;
        [Header("Room Doorway Places")]
        [SerializeField] private List<DoorWayView> _doorWayViews;
        [Header("Spawn Locations")]
        [SerializeField] private Transform[] _trapSpawnpoints;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [Header("Confiner Zone")]
        [SerializeField] private BoxCollider _confiner;
        [Header("NavMesh")]
        [SerializeField] private NavMeshSurface _navSurface;
        [Header("ComplitSprite")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Sprite _complitIcon;

        private List<RoomDoorView> _roomDoorViews =  new ();
        private RoomDoorView _openDoor;
        private int _countEnemy;
        private int _dividerIndexColor = 2;

        public event Action<RoomView> RoomEntering;

        public RemovableWall WallUpper => _removableWalls.SingleOrDefault(wall => wall.TypeRoomSide == TypeRoomSide.Upper);
        public RemovableWall WallRight => _removableWalls.SingleOrDefault(wall => wall.TypeRoomSide == TypeRoomSide.Right);
        public RemovableWall WallDown => _removableWalls.SingleOrDefault(wall => wall.TypeRoomSide == TypeRoomSide.Down);
        public RemovableWall WallLeft => _removableWalls.SingleOrDefault(wall => wall.TypeRoomSide == TypeRoomSide.Left);
        public BoxCollider Confiner => _confiner;
        public Transform[] EnemySpawnPoints => _enemySpawnPoints;
        public Transform[] TrapSpawnPoints => _trapSpawnpoints;
        public RoomData RoomData { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsComplete { get; private set; } = false;
        public int CountEnemy => _countEnemy;
        public Sprite IconRoom => _renderer.sprite;

        private void OnDestroy()
        {
            foreach (var door in _roomDoorViews)
            {
                door.RoomDoorTaked -= OnDoorTaked;
            }

            foreach (DoorWayView doorWayView in _doorWayViews)
            {
                if(doorWayView != null)
                    Destroy(doorWayView.gameObject);
            }

            foreach (RemovableWall removableWall in _removableWalls)
            {
                if (removableWall != null)
                    Destroy(removableWall.gameObject);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Player player))
                RoomEntering?.Invoke(this);
        }

        public void SetRoomStatus()
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

            if (RoomData.Id == 1)
                _countEnemy = 1;
            else
                _countEnemy = 15; //tests

            CreateDoorway();
            AddListener();
            RemoveUnusedDoors();
            RemoveUnusedWalls();
            UpdateMaterialColor(roomData.TierColor[currentLevel / _dividerIndexColor]);
        }

        public void SetComplete()
        {
            IsComplete = true;
            UnlockRoom();
            SetRoomComplitIcon();
        }

        public void LockRoom()
        {
            if (IsComplete == true)
                return;

            foreach (var door in _roomDoorViews)
            {
                door.Lock();
            }
        }

        public virtual void UnlockRoom()
        {
            foreach (var door in _roomDoorViews)
            {
                door.Unlock();
            }

            if (_openDoor != null)
                _openDoor.SetDoorOpen();
        }

        public void ResetAllRemovableWall()
        {
            foreach (RemovableWall removableWall in _removableWalls)
            {
                if (removableWall != null)
                    removableWall.gameObject.SetActive(true);
            }
        }

        protected void SetRoomComplitIcon()
        {
            _renderer.sprite = _complitIcon;
        }

        private void AddListener() 
        {
            foreach (var door in _roomDoorViews)
            {
                door.RoomDoorTaked += OnDoorTaked;
            }
        }

        private void UpdateMaterialColor(Color color) 
        {
            foreach (Wall wall in _walls)
            {
                wall.UpdateMaterial(color);
            }

            foreach (Wall wall in _removableWalls)
            {
                wall.UpdateMaterial(color);
            }

            foreach (Light light in _lights) 
            {
                light.color = color;
            }
        }

        private void CreateDoorway() 
        {
            foreach (RemovableWall removableWall in _removableWalls) 
            {
                foreach (DoorWayView doorWayView in _doorWayViews) 
                {
                    if (removableWall.gameObject != null && removableWall.gameObject.activeSelf == false && removableWall.TypeRoomSide == doorWayView.TypeRoomSide)
                    {
                        doorWayView.gameObject.SetActive(true);
                        _roomDoorViews.Add(doorWayView.RoomDoor);
                    }
                }
            }
        }

        private void RemoveUnusedDoors() 
        {
            foreach (DoorWayView doorWayView in _doorWayViews)
            {
                if (doorWayView.gameObject.activeSelf == false)
                    Destroy(doorWayView.gameObject);
            }
        }

        private void RemoveUnusedWalls()
        {
            foreach (RemovableWall removableWall in _removableWalls)
            {
                if (removableWall.gameObject.activeSelf == false)
                    Destroy(removableWall.gameObject);
            }
        }

        private void OnDoorTaked(RoomDoorView roomDoor) 
        {
            _openDoor = roomDoor;
        }
    }
}