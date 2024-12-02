using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class RoomPlacer : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly int _roomSize = 40;
        private readonly int _maxRoomCount = 12;
        private readonly int _massRoomSize = 11;
        private readonly int _spawnCenterCoordinate = 5;
        private readonly int _minValue = 0;
        private readonly int _shiftIndex = 1;
        private readonly int _roomBossId = 1;

        [SerializeField] private RoomData[] _roomDatas;
        [SerializeField] private RoomData _defaultRoomData;
        [SerializeField] private Room _startRoom;
        [SerializeField] private Transform _playerSpawnPoint;

        private Room[,] _spawnedRooms;
        private List<Room> _createdRooms = new ();
        private int _allEnemyCount;

        public List<Room> CreatedRooms => _createdRooms;
        public Room StartRoom => _startRoom;
        public int AllEnemyCount => _allEnemyCount;

        public void Initialize(int currentRoomLevel, bool canSeeDoor)
        {
            _spawnedRooms = new Room[_massRoomSize, _massRoomSize];
            _spawnedRooms[_spawnCenterCoordinate, _spawnCenterCoordinate] = _startRoom;
            _startRoom.SetCameraArial(canSeeDoor);

            for (int index = 0; index < _maxRoomCount; index++)
            {
                PlaceOneRoom(currentRoomLevel, canSeeDoor);
            }
        }

        private void PlaceOneRoom(int currentRoomLevel, bool canSeeDoor)
        {
            HashSet<Vector2Int> freeSpawnSpace = new();

            for (int x = 0; x < _spawnedRooms.GetLength(_minValue); x++)
            {
                for (int y = 0; y < _spawnedRooms.GetLength(_shiftIndex); y++)
                {
                    if (_spawnedRooms[x, y] == null)
                        continue;

                    int maxX = _spawnedRooms.GetLength(_minValue) - _shiftIndex;
                    int maxY = _spawnedRooms.GetLength(_shiftIndex) - _shiftIndex;

                    if (x > 0 && _spawnedRooms[x - _shiftIndex, y] == null)
                        freeSpawnSpace.Add(new Vector2Int(x - _shiftIndex, y));

                    if (y > 0 && _spawnedRooms[x, y - _shiftIndex] == null)
                        freeSpawnSpace.Add(new Vector2Int(x, y - _shiftIndex));

                    if (x < maxX && _spawnedRooms[x + _shiftIndex, y] == null)
                        freeSpawnSpace.Add(new Vector2Int(x + _shiftIndex, y));

                    if (y < maxY && _spawnedRooms[x, y + _shiftIndex] == null)
                        freeSpawnSpace.Add(new Vector2Int(x, y + _shiftIndex));
                }
            }

            RoomData randomRoomData = GetRandomRoom();

            if (TryGetBossRoom(randomRoomData))
                randomRoomData = _defaultRoomData;

            Room newRoom = Instantiate(randomRoomData.Room);
            int limit = 500; //значение для теста

            while (limit-- > 0)
            {
                Vector2Int position = freeSpawnSpace.ElementAt(_rnd.Next(0, freeSpawnSpace.Count));
                //newRoom.RotateWallRandomly(); // временно убран

                if (ConnectRooms(newRoom, position))
                {
                    newRoom.transform.position = new Vector3(position.x - _spawnCenterCoordinate, 0, position.y - _spawnCenterCoordinate) * _roomSize;
                    _spawnedRooms[position.x, position.y] = newRoom;
                    newRoom.Initialize(randomRoomData, currentRoomLevel, canSeeDoor);
                    _allEnemyCount += newRoom.CountEnemy;
                    _createdRooms.Add(newRoom);
                    return;
                }
            }

            Destroy(newRoom.gameObject);
        }

        private bool ConnectRooms(Room room, Vector2Int vector2)
        {
            int maxX = _spawnedRooms.GetLength(_minValue) - _shiftIndex;
            int maxY = _spawnedRooms.GetLength(_shiftIndex) - _shiftIndex;

            List<Vector2Int> neighbours = new ();

            if (room.WallUpper != null && vector2.y < maxY && _spawnedRooms[vector2.x, vector2.y + _shiftIndex]?.WallDown != null)
                neighbours.Add(Vector2Int.up);

            if (room.WallDown != null && vector2.y > 0 && _spawnedRooms[vector2.x, vector2.y - _shiftIndex]?.WallUpper != null)
                neighbours.Add(Vector2Int.down);

            if (room.WallRight != null && vector2.x < maxX && _spawnedRooms[vector2.x + _shiftIndex, vector2.y]?.WallLeft != null)
                neighbours.Add(Vector2Int.right);

            if (room.WallLeft != null && vector2.x > 0 && _spawnedRooms[vector2.x - _shiftIndex, vector2.y]?.WallRight != null)
                neighbours.Add(Vector2Int.left);

            if (neighbours.Count == 0)
                return false;

            Vector2Int selectedDirection = neighbours[_rnd.Next(0, neighbours.Count)];
            Room selectedRoom = _spawnedRooms[vector2.x + selectedDirection.x, vector2.y + selectedDirection.y];

            if (selectedDirection == Vector2Int.up)
            {
                room.WallUpper.SetActive(false);
                selectedRoom.WallDown.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.down)
            {
                room.WallDown.SetActive(false);
                selectedRoom.WallUpper.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.right)
            {
                room.WallRight.SetActive(false);
                selectedRoom.WallLeft.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.left)
            {
                room.WallLeft.SetActive(false);
                selectedRoom.WallRight.SetActive(false);
            }

            return true;
        }

        private bool TryGetBossRoom(RoomData randomRoomData) 
        {
            if (randomRoomData.Id == _roomBossId)
            {
                foreach (var room in _createdRooms)
                {
                    if (room.RoomData.Id == randomRoomData.Id)
                        return true;
                }
            }

            return false;
        }

        private RoomData GetRandomRoom()
        {
            List<float> chances = new ();

            for (int index = 0; index < _roomDatas.Length; index++)
            {
                chances.Add(_roomDatas[index].ChanceFromDistance.Evaluate(_playerSpawnPoint.position.x));
            }

            float value = Random.Range(0, chances.Sum());
            float sum = 0;

            for (int index = 0; index < chances.Count; index++)
            {
                sum += chances[index];

                if (value < sum)
                    return _roomDatas[index];
            }

            return _roomDatas[_roomDatas.Length - _shiftIndex];
        }
    }
}