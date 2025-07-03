using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class RoomPlacer : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly int _roomSize = 40;
        private readonly int _minValue = 0;
        private readonly int _shiftIndex = 1;
        private readonly int _multiplierRoomSize = 2;

        [SerializeField] private RoomData[] _roomDatas;
        [SerializeField] private RoomData _bossRoomData;
        [SerializeField] private RoomData _defaultRoomData;
        [SerializeField] private RoomView _startRoom;
        [SerializeField] private Transform _playerSpawnPoint;

        private int _spawnCenterCoordinate = 5;
        private int _maxRoomSize = 11;
        private RoomView[,] _spawnedRooms;
        private List<RoomView> _createdRooms = new ();
        private int _maxRoomCount = 0;

        public List<RoomView> CreatedRooms => _createdRooms;
        public RoomView StartRoom => _startRoom;

        public void Initialize(int currentRoomLevel, bool canSeeDoor, int countRooms, CameraControiler cameraControiler)
        {
            _startRoom.SetCameraArial(canSeeDoor);
            _maxRoomSize = (countRooms * _multiplierRoomSize) + _shiftIndex;
            _spawnCenterCoordinate = countRooms;
            _maxRoomCount = countRooms;
            _spawnedRooms = new RoomView[_maxRoomSize, _maxRoomSize];
            _spawnedRooms[_spawnCenterCoordinate, _spawnCenterCoordinate] = _startRoom;
            cameraControiler.ChangeConfiner(_startRoom);

            for (int index = 0; index < _maxRoomCount; index++)
            {
                PlaceOneRoom(currentRoomLevel, canSeeDoor, index);
            }
        }

        public void Clear() 
        {
            foreach (var room in _createdRooms) 
            {
                Destroy(room.gameObject);
            }

            _createdRooms.Clear();
            _startRoom.ResetAllRemovableWall();
        }

        private void PlaceOneRoom(int currentRoomLevel, bool canSeeDoor, int roomIndex)
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

            if (roomIndex + _shiftIndex == _maxRoomCount)
                randomRoomData =  _bossRoomData;

            RoomView newRoom = Instantiate(randomRoomData.Room);
            int limitRoomPlace = 500;

            while (limitRoomPlace-- > 0)
            {
                Vector2Int position = freeSpawnSpace.ElementAt(_rnd.Next(0, freeSpawnSpace.Count));

                if (ConnectRooms(newRoom, position))
                {
                    newRoom.transform.position = new Vector3(position.x - _spawnCenterCoordinate, 0, position.y - _spawnCenterCoordinate) * _roomSize;
                    _spawnedRooms[position.x, position.y] = newRoom;
                    newRoom.Initialize(randomRoomData, currentRoomLevel, canSeeDoor);
                    _createdRooms.Add(newRoom);
                    return;
                }
            }

            Destroy(newRoom.gameObject);
        }

        private bool ConnectRooms(RoomView room, Vector2Int vector2)
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
            RoomView selectedRoom = _spawnedRooms[vector2.x + selectedDirection.x, vector2.y + selectedDirection.y];

            if (selectedDirection == Vector2Int.up)
            {
                room.WallUpper.gameObject.SetActive(false);
                selectedRoom.WallDown.gameObject.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.down)
            {
                room.WallDown.gameObject.SetActive(false);
                selectedRoom.WallUpper.gameObject.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.right)
            {
                room.WallRight.gameObject.SetActive(false);
                selectedRoom.WallLeft.gameObject.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.left)
            {
                room.WallLeft.gameObject.SetActive(false);
                selectedRoom.WallRight.gameObject.SetActive(false);
            }

            return true;
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