using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Factories;
using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.SpawnersScripts
{
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
            CameraScripts.CameraController cameraControiler,
            Player playerPrefab,
            PlayerView playerView,
            Transform spawnPoint,
            AudioPlayer audioPlayer,
            out Player spawnedPlayer)
        {
            _spawnPoint = spawnPoint;
            _playerClassData = gameConfig.GetPlayerClassDataById(
                persistentDataService.PlayerProgress.CurrentPlayerClassId);
            _playerPrefab = playerPrefab;
            spawnedPlayer = Object.Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);

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
}