using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class RoomPlacer : MonoBehaviour
    {
        private readonly int _roomSize = 16;
        private readonly int _maxRoomCount = 12;
        private readonly int _massRoomSize = 11;
        private readonly int _spawnCenterCoordinate = 5;
        private readonly int _minValue = 0;
        private readonly int _shitIndex = 1;

        [SerializeField] private RoomData[] _roomDatas;
        [SerializeField] private Room _startRoom;

        private Room[,] spawnedRooms;

        private void Start()
        {
            spawnedRooms = new Room[_massRoomSize, _massRoomSize];
            spawnedRooms[_spawnCenterCoordinate, _spawnCenterCoordinate] = _startRoom;

            for (int index = 0; index < _maxRoomCount; index++)
            {
                PlaceOneRoom();
            }
        }

        private void PlaceOneRoom()
        {
            HashSet<Vector2Int> freeSpawnSpace = new ();

            for (int x = 0; x < spawnedRooms.GetLength(_minValue); x++)
            {
                for (int y = 0; y < spawnedRooms.GetLength(_shitIndex); y++)
                {
                    if (spawnedRooms[x, y] == null) 
                        continue;

                    int maxX = spawnedRooms.GetLength(_minValue) - _shitIndex;
                    int maxY = spawnedRooms.GetLength(_shitIndex) - _shitIndex;

                    if (x > 0 && spawnedRooms[x - _shitIndex, y] == null) 
                        freeSpawnSpace.Add(new Vector2Int(x - _shitIndex, y));

                    if (y > 0 && spawnedRooms[x, y - _shitIndex] == null) 
                        freeSpawnSpace.Add(new Vector2Int(x, y - _shitIndex));

                    if (x < maxX && spawnedRooms[x + _shitIndex, y] == null) 
                        freeSpawnSpace.Add(new Vector2Int(x + _shitIndex, y));

                    if (y < maxY && spawnedRooms[x, y + _shitIndex] == null) 
                        freeSpawnSpace.Add(new Vector2Int(x, y + _shitIndex));
                }
            }

            Room newRoom = Instantiate(_roomDatas[Random.Range(0, _roomDatas.Length)].Room); // заменить на GetRandomRoom  + доавить вероятность комнаты

            int limit = 500; //значение для теста

            while (limit-- > 0)
            {
                // Эту строчку можно заменить на выбор положения комнаты с учётом того насколько он далеко/близко от центра,
                // или сколько у него соседей, чтобы генерировать более плотные, или наоборот, растянутые данжи
                Vector2Int position = freeSpawnSpace.ElementAt(Random.Range(0, freeSpawnSpace.Count));
                newRoom.RotateRandomly();

                if (ConnectRooms(newRoom, position))
                {
                    newRoom.transform.position = new Vector3(position.x - _spawnCenterCoordinate, 0, position.y - _spawnCenterCoordinate) * _roomSize;
                    spawnedRooms[position.x, position.y] = newRoom;
                    return;
                }
            }

            Destroy(newRoom.gameObject);
        }

        private bool ConnectRooms(Room room, Vector2Int vector2)
        {
            int maxX = spawnedRooms.GetLength(_minValue) - _shitIndex;
            int maxY = spawnedRooms.GetLength(_shitIndex) - _shitIndex;

            List<Vector2Int> neighbours = new ();

            if (room.DoorUpper != null && vector2.y < maxY && spawnedRooms[vector2.x, vector2.y + _shitIndex]?.DoorDown != null) 
                neighbours.Add(Vector2Int.up);

            if (room.DoorDown != null && vector2.y > 0 && spawnedRooms[vector2.x, vector2.y - _shitIndex]?.DoorUpper != null) 
                neighbours.Add(Vector2Int.down);

            if (room.DoorRight != null && vector2.x < maxX && spawnedRooms[vector2.x + _shitIndex, vector2.y]?.DoorLeft != null) 
                neighbours.Add(Vector2Int.right);

            if (room.DoorLeft != null && vector2.x > 0 && spawnedRooms[vector2.x - _shitIndex, vector2.y]?.DoorRight != null) 
                neighbours.Add(Vector2Int.left);

            if (neighbours.Count == 0) 
                return false;

            Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];
            Room selectedRoom = spawnedRooms[vector2.x + selectedDirection.x, vector2.y + selectedDirection.y];

            if (selectedDirection == Vector2Int.up)
            {
                room.DoorUpper.SetActive(false);
                selectedRoom.DoorDown.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.down)
            {
                room.DoorDown.SetActive(false);
                selectedRoom.DoorUpper.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.right)
            {
                room.DoorRight.SetActive(false);
                selectedRoom.DoorLeft.SetActive(false);
            }
            else if (selectedDirection == Vector2Int.left)
            {
                room.DoorLeft.SetActive(false);
                selectedRoom.DoorRight.SetActive(false);
            }

            return true;
        }

        private Room GetRandomRoom()
        {
            List<float> chances = new ();

            for (int index = 0; index < _roomDatas.Length; index++)
            {
               // chances.Add(_roomDatas[i].Room.ChanceFromDistance.Evaluate(Player.transform.position.z)); //Player Transform или точка спавна персонажа
            }

            float value = Random.Range(0, chances.Sum());
            float sum = 0;

            for (int index = 0; index < chances.Count; index++)
            {
                sum += chances[index];

                if (value < sum)
                    return _roomDatas[index].Room;
            }

            return _roomDatas[_roomDatas.Length - _shitIndex].Room;
        }
    }
}