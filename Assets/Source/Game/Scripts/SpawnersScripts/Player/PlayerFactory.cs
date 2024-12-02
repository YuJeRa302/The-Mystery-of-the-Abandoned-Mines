using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint; //���������
    [SerializeField] private WeaponData _weaponData; //����� ����� ������������ ��� ������������� ��������� �����, ������� ��� ��������
    [SerializeField] private PlayerClassData _classData; //����� ����� ������������ ��� ������������� ��������� �����, ������� ��� ��������
    [SerializeField] private Player _playerPrefab;

    public void SpawnPlayer(out Player spawnedPlayer)
    {
        spawnedPlayer = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        spawnedPlayer.WeaponView.Initialize(spawnedPlayer, _weaponData);
        spawnedPlayer.PlayerAnimation.Initialize(_classData);
    }
}