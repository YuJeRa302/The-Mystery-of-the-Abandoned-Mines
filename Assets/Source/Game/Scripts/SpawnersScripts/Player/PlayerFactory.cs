using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory
{
    private readonly Transform _spawnPoint;
    private readonly PlayerClassData _playerClassData;
    private readonly Player _playerPrefab;

    public PlayerFactory(
        AbilityFactory abilityFactory,
        AbilityPresenterFactory abilityPresenterFactory,
        PersistentDataService persistentDataService,
        GamePauseService gamePauseService,
        GameConfig gameConfig,
        CameraControiler cameraControiler,
        Player playerPrefab,
        PlayerView playerView,
        Transform spawnPoint,
        AudioPlayer audioPlayer,
        out Player spawnedPlayer)
    {
        _spawnPoint = spawnPoint;
        _playerClassData = gameConfig.GetPlayerClassDataById(persistentDataService.PlayerProgress.CurrentPlayerClassId);
        _playerPrefab = playerPrefab;
        spawnedPlayer = GameObject.Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);

        spawnedPlayer.CreatePlayerEntities(
            abilityFactory,
            abilityPresenterFactory,
            persistentDataService,
            gamePauseService,
            gameConfig,
            cameraControiler,
            playerView,
            _playerClassData,
            spawnPoint,
            audioPlayer);
    }
}