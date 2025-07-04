using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapsSpawner : IDisposable
{
    private readonly System.Random _rnd = new ();

    private Transform[] _spawnPoints;
    private GameObject[] _traps;
    private List<GameObject> _spawnedTraps = new ();
    private LootRoomView _currentRoom;

    public TrapsSpawner ()
    {
    }

    public void Initialize(RoomView room)
    {
        if (_currentRoom != null)
            _currentRoom.RoomCompleted -= OnDisableTraps;

        _spawnedTraps.Clear();
        _currentRoom = room as LootRoomView;
        
        if (_currentRoom != null)
        {
            _currentRoom.RoomCompleted += OnDisableTraps;
            _spawnPoints = _currentRoom.TrapSpawnPoints;
            _traps = _currentRoom.RoomData.Traps;
            SpawnTraps();
        }
    }

    public void Dispose()
    {
        if (_currentRoom != null)
            _currentRoom.RoomCompleted -= OnDisableTraps;

        GC.SuppressFinalize(this);
    }

    private void SpawnTraps()
    {
        foreach (var point in _spawnPoints)
        {
            _spawnedTraps.Add(GameObject.Instantiate(_traps[_rnd.Next(_traps.Length)], point));
        }
    }

    private void OnDisableTraps()
    {
        foreach (var trap in _spawnedTraps)
        {
            trap.gameObject.SetActive(false);
        }
    }
}