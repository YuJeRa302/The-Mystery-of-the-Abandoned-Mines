using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class TrapsSpawner : IDisposable
{
    private readonly System.Random _rnd = new ();

    private Transform[] _spawnPoints;
    private GameObject[] _traps;
    private RoomView _currentRoom;

    public TrapsSpawner ()
    {
    }

    public void Initialize(RoomView room)
    {
        _currentRoom = room;
        _spawnPoints = _currentRoom.TrapSpawnPoints;
        _traps = _currentRoom.RoomData.Traps;
        SpawnTraps();
    }

    private void SpawnTraps()
    {
        foreach (var point in _spawnPoints)
        {
            GameObject.Instantiate(_traps[_rnd.Next(_traps.Length)], point);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}