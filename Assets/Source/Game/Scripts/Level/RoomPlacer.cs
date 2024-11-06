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
        private readonly int _shitIndex = 1;

        [SerializeField] private RoomData[] _roomDatas;
        [SerializeField] private Room _startRoom;
        [SerializeField] private Transform _playerSpawnPoint;

        private Room[,] _spawnedRooms;
        private List<Room> _createdRooms = new ();

        public List<Room> CreatedRooms => _createdRooms;
        public Room StartRoom => _startRoom;

        public void Initialize(int currentRoomLevel)
        {
            _spawnedRooms = new Room[_massRoomSize, _massRoomSize];
            _spawnedRooms[_spawnCenterCoordinate, _spawnCenterCoordinate] = _startRoom;

            for (int index = 0; index < _maxRoomCount; index++)
            {
                PlaceOneRoom(currentRoomLevel);
            }
        }

        private void PlaceOneRoom(int currentRoomLevel)
        {
            HashSet<Vector2Int> freeSpawnSpace = new();

            for (int x = 0; x < _spawnedRooms.GetLength(_minValue); x++)
            {
                for (int y = 0; y < _spawnedRooms.GetLength(_shitIndex); y++)
                {
                    if (_spawnedRooms[x, y] == null)
                        continue;

                    int maxX = _spawnedRooms.GetLength(_minValue) - _shitIndex;
                    int maxY = _spawnedRooms.GetLength(_shitIndex) - _shitIndex;

                    if (x > 0 && _spawnedRooms[x - _shitIndex, y] == null)
                        freeSpawnSpace.Add(new Vector2Int(x - _shitIndex, y));

                    if (y > 0 && _spawnedRooms[x, y - _shitIndex] == null)
                        freeSpawnSpace.Add(new Vector2Int(x, y - _shitIndex));

                    if (x < maxX && _spawnedRooms[x + _shitIndex, y] == null)
                        freeSpawnSpace.Add(new Vector2Int(x + _shitIndex, y));

                    if (y < maxY && _spawnedRooms[x, y + _shitIndex] == null)
                        freeSpawnSpace.Add(new Vector2Int(x, y + _shitIndex));
                }
            }

            RoomData randomRoomData = GetRandomRoom();
            Room newRoom = Instantiate(randomRoomData.Room);

            int limit = 500; //значение для теста

            while (limit-- > 0)
            {
                // Эту строчку можно заменить на выбор положения комнаты с учётом того насколько он далеко/близко от центра,
                // или сколько у него соседей, чтобы генерировать более плотные, или наоборот, растянутые данжи
                Vector2Int position = freeSpawnSpace.ElementAt(_rnd.Next(0, freeSpawnSpace.Count));
                //newRoom.RotateWallRandomly(); // временно убран

                if (ConnectRooms(newRoom, position))
                {
                    newRoom.transform.position = new Vector3(position.x - _spawnCenterCoordinate, 0, position.y - _spawnCenterCoordinate) * _roomSize;
                    _spawnedRooms[position.x, position.y] = newRoom;
                    newRoom.Initialize(randomRoomData, currentRoomLevel);
                    _createdRooms.Add(newRoom);
                    return;
                }
            }

            Destroy(newRoom.gameObject);
        }

        private bool ConnectRooms(Room room, Vector2Int vector2)
        {
            int maxX = _spawnedRooms.GetLength(_minValue) - _shitIndex;
            int maxY = _spawnedRooms.GetLength(_shitIndex) - _shitIndex;

            List<Vector2Int> neighbours = new ();

            if (room.WallUpper != null && vector2.y < maxY && _spawnedRooms[vector2.x, vector2.y + _shitIndex]?.WallDown != null)
                neighbours.Add(Vector2Int.up);

            if (room.WallDown != null && vector2.y > 0 && _spawnedRooms[vector2.x, vector2.y - _shitIndex]?.WallUpper != null)
                neighbours.Add(Vector2Int.down);

            if (room.WallRight != null && vector2.x < maxX && _spawnedRooms[vector2.x + _shitIndex, vector2.y]?.WallLeft != null)
                neighbours.Add(Vector2Int.right);

            if (room.WallLeft != null && vector2.x > 0 && _spawnedRooms[vector2.x - _shitIndex, vector2.y]?.WallRight != null)
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

            return _roomDatas[_roomDatas.Length - _shitIndex];
        }
    }
}