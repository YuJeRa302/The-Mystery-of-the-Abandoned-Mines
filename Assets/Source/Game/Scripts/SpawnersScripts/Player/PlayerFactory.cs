using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory
{
    private Transform _spawnPoint;
    private PlayerClassData _classData;
    private Player _playerPrefab;
    
    private WeaponData _weaponData;

    public PlayerFactory(PlayerInventory inventory, LevelObserver observer, AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory, 
        Player playerPrefab, Transform spawnPoint, PlayerClassData classData, out Player spawnedPlayer)
    {
        _spawnPoint = spawnPoint;
        _classData = classData;
        _playerPrefab = playerPrefab;
        spawnedPlayer = GameObject.Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        //_weaponData = inventory.WarlockWeapons[0];//
        //spawnedPlayer.Equipment.Initialize(spawnedPlayer, inventory.WarlockWeapons[0]);
        //spawnedPlayer.PlayerAnimation.Initialize(_classData);
        spawnedPlayer.CreateStats(observer, classData, inventory.WarlockWeapons[0]);
        spawnedPlayer.PlayerStats.Initialize(0, null, observer, abilityFactory, abilityPresenterFactory);
    }
}