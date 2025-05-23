using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory
{
    private Transform _spawnPoint;
    private PlayerClassData _classData;
    private Player _playerPrefab;
    
    private WeaponData _weaponData;

    public PlayerFactory(
        
        LevelObserver observer,
        AbilityFactory abilityFactory,
        AbilityPresenterFactory abilityPresenterFactory, 
        Player playerPrefab,
        Transform spawnPoint,
        PlayerView playerView,
        TemporaryData temporaryData,
        AudioPlayer audioPlayer,
        out Player spawnedPlayer)
    {
        _spawnPoint = spawnPoint;
        _classData = temporaryData.PlayerClassData;
        _playerPrefab = playerPrefab;
        spawnedPlayer = GameObject.Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        spawnedPlayer.CreateStats(observer, _classData, playerView, temporaryData.WeaponData, abilityFactory, abilityPresenterFactory, temporaryData, audioPlayer);
    }
}