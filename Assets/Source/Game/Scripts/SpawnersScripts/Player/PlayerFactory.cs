using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint; //перенести
    [SerializeField] private WeaponData _weaponData; //далее будет передоваться при инициализации выбранный вепон, проходя все проверки
    [SerializeField] private PlayerClassData _classData; //далее будет передоваться при инициализации выбранный вепон, проходя все проверки
    [SerializeField] private Player _playerPrefab;

    public void SpawnPlayer(out Player spawnedPlayer)
    {
        spawnedPlayer = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        spawnedPlayer.WeaponView.Initialize(spawnedPlayer, _weaponData);
        spawnedPlayer.PlayerAnimation.Initialize(_classData);
    }
}