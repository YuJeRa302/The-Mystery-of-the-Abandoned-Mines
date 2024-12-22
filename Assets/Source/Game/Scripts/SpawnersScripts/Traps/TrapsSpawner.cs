using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class TrapsSpawner : IDisposable
{
    private readonly System.Random _rnd = new();

    private Transform[] _spawnPoints;
    private GameObject[] _traps;
    private Room _currentRoom;

    public TrapsSpawner ()
    {
    }

    public void Initialize(Room room)
    {
        if (_currentRoom != null)
            _currentRoom.PlateDiscovery.PlateEntered -= OnOpenRoom;

        _currentRoom = room;
        _spawnPoints = _currentRoom.TrapSpawnPoints;
        _traps = _currentRoom.RoomData.Traps;
        _currentRoom.PlateDiscovery.PlateEntered += OnOpenRoom;
        SpawnTraps();
    }

    private void SpawnTraps()
    {
        foreach (var point in _spawnPoints)
        {
            GameObject.Instantiate(_traps[_rnd.Next(_traps.Length)], point);
        }
    }

    private void OnOpenRoom()
    {
        _currentRoom.UnlockRoom();
    }

    public void Dispose()
    {
        if (_currentRoom != null)
            _currentRoom.PlateDiscovery.PlateEntered -= OnOpenRoom;

        GC.SuppressFinalize(this);
    }
}