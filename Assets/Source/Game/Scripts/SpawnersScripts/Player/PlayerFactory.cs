using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory
{
    private Transform _spawnPoint;
    private PlayerClassData _classData;
    private Player _playerPrefab;
    
    private WeaponData _weaponData;

    public PlayerFactory(WeaponData weapon, LevelObserver observer, AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory, 
        Player playerPrefab, Transform spawnPoint, PlayerClassData classData, TemporaryData temporaryData, out Player spawnedPlayer)
    {
        _spawnPoint = spawnPoint;
        _classData = classData;
        _playerPrefab = playerPrefab;
        spawnedPlayer = GameObject.Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        spawnedPlayer.CreateStats(observer, classData, weapon, abilityFactory, abilityPresenterFactory, temporaryData);
    }
}