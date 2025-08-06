using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Rooms;
using Assets.Source.Game.Scripts.SpawnersScripts;
using Assets.Source.Game.Scripts.Views;
using System;
using System.Linq;

namespace Assets.Source.Game.Scripts.Services
{
    public class RoomService : IDisposable
    {
        private readonly GamePanelsService _gamePanelsService;
        private readonly TrapsSpawner _trapsSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly RoomPlacer _roomPlacer;
        private readonly float _minCountRooms = 1;

        private Player _player;
        private CameraScripts.CameraController _cameraControiler;
        private RoomView _currentRoom;
        private int _countRooms;
        private bool _canSeeDoor;
        private int _countStages = 0;
        private int _currentStage = 0;

        public RoomService(
            GamePanelsService gamePanelsService,
            RoomPlacer roomPlacer,
            CameraScripts.CameraController cameraControiler,
            EnemySpawner enemySpawner,
            TrapsSpawner trapsSpawner,
            int countRooms,
            int countStages)
        {
            _gamePanelsService = gamePanelsService;
            _roomPlacer = roomPlacer;
            _countRooms = countRooms;
            _countStages = countStages;
            _cameraControiler = cameraControiler;
            _enemySpawner = enemySpawner;
            _trapsSpawner = trapsSpawner;
            _canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft.gameObject);
            _roomPlacer.Initialize(CurrentRoomLevel, _canSeeDoor, _countRooms, _cameraControiler);
            AddRoomListener();
            LockBossRoom();
        }

        public event Action StageCompleted;
        public event Action<int> LootRoomCompleted;
        public event Action<bool> GameEnded;

        public bool IsWinGame { get; private set; } = false;
        public bool IsGameInterrupted { get; private set; } = true;
        public int CurrentRoomLevel { get; private set; } = 0;

        public void Dispose()
        {
            RemoveListener();
            RemoveRoomListener();
        }

        public void InitPlayerInstance(Player player)
        {
            _player = player;
            _cameraControiler.SetLookTarget(_player.transform);
            AddListener();
        }

        private void ClearRooms()
        {
            _roomPlacer.Clear();
            _enemySpawner.ClearEnemyInRoom();
        }

        private void CreateNextStage()
        {
            RemoveRoomListener();
            _roomPlacer.Clear();
            CurrentRoomLevel++;
            _roomPlacer.Initialize(CurrentRoomLevel, _canSeeDoor, _countRooms, _cameraControiler);
            _player.ResetPosition();
            StageCompleted?.Invoke();
            AddRoomListener();
            LockBossRoom();
        }

        private void StageComplete()
        {
            _currentStage++;

            if (_currentStage == _countStages)
                WinGame();
            else
                CreateNextStage();
        }

        private void AddListener()
        {
            _enemySpawner.AllEnemyRoomDied += OnRoomCompleted;
            _enemySpawner.EnemyDied += OnEnemyDied;
            _player.PlayerDied += OnPlayerDied;
        }

        private void RemoveListener()
        {
            _enemySpawner.AllEnemyRoomDied -= OnRoomCompleted;
            _enemySpawner.EnemyDied -= OnEnemyDied;
            _player.PlayerDied -= OnPlayerDied;
        }

        private void AddRoomListener()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering += OnRoomEntering;

                if (room == room as LootRoomView)
                {
                    (room as LootRoomView).RoomCompleted += OnRoomCompleted;
                    (room as LootRoomView).RewardSeted += OnLootRoomCompleted;
                }
            }

            _roomPlacer.StartRoom.SetRoomStatus();
            _roomPlacer.StartRoom.RoomEntering += OnRoomEntering;
        }

        private void RemoveRoomListener()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                if (room == null)
                    continue;

                room.RoomEntering -= OnRoomEntering;

                if (room == room as LootRoomView)
                {
                    (room as LootRoomView).RoomCompleted -= OnRoomCompleted;
                    (room as LootRoomView).RewardSeted -= OnLootRoomCompleted;
                }
            }
        }

        private void LockAllDoors()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.LockRoom();
            }
        }

        private void LockBossRoom()
        {
            BossRoomView bossRoom = null;

            foreach (RoomView room in _roomPlacer.CreatedRooms)
            {
                if (room as BossRoomView)
                    bossRoom = room as BossRoomView;
            }

            if (_countRooms > _minCountRooms)
                bossRoom.LockRoom();
            else
                bossRoom.UnlockBossRoom(true);
        }

        private void UnlockAllDoors()
        {
            bool isAllRoomsComplete = _roomPlacer.CreatedRooms
            .Where(room => !(room is BossRoomView))
            .All(room => room.IsComplete);

            foreach (var room in _roomPlacer.CreatedRooms)
            {
                if (room == room as BossRoomView)
                    (room as BossRoomView).UnlockBossRoom(isAllRoomsComplete);
                else
                    room.UnlockRoom();
            }
        }

        private void WinGame()
        {
            _gamePanelsService.ClosePanels();
            IsWinGame = true;
            IsGameInterrupted = false;
            ClearRooms();
            GameEnded?.Invoke(IsWinGame);
        }

        private void OnPlayerDied()
        {
            _gamePanelsService.ClosePanels();
            IsGameInterrupted = false;
            ClearRooms();
            GameEnded?.Invoke(IsWinGame);
        }

        private void OnEnemyDied(Enemy enemy)
        {
            _player.PlayerStats.EnemyDied(enemy);
        }

        private void OnLootRoomCompleted(int reward)
        {
            LootRoomCompleted?.Invoke(reward);
        }

        private void OnRoomCompleted()
        {
            if (_currentRoom != _currentRoom as BossRoomView)
            {
                _currentRoom.SetComplete();
                UnlockAllDoors();
            }

            if (_currentRoom == _currentRoom as BossRoomView)
                StageComplete();
        }

        private void OnRoomEntering(RoomView room)
        {
            _currentRoom = room;
            _cameraControiler.ChangeConfiner(room);

            if (room.IsComplete == false)
            {
                if (room.EnemySpawnPoints.Length == 0)
                    _trapsSpawner.Initialize(room);
                else
                    _enemySpawner.EnterRoom(room);

                LockAllDoors();
            }
        }
    }
}