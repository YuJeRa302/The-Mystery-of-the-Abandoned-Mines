using Assets.Source.Game.Scripts;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class RoomKnowladgeView : KnowladgeView
{
    [SerializeField] private Image _roomIcon;
    [SerializeField] private LeanLocalizedText _roomName;
    [SerializeField] private LeanLocalizedText _roomDescription;
    [SerializeField] private Transform _spawnedEnemyConteiner;
    [SerializeField] private SpawnedEnemyView _spawnedEnemyView;

    public void Initialize(RoomData roomData)
    {
        _roomIcon.sprite = roomData.Room.IconRoom;
        _roomName.TranslationName = roomData.RoomName;
        _roomDescription.TranslationName = roomData.RoomDescription;

        SpawnEnemyView(roomData);
    }

    private void SpawnEnemyView(RoomData roomData)
    {
        if (roomData.EnemyDatas.Length > 1)
        {
            foreach (var enemy in roomData.EnemyDatas)
            {
                SpawnedEnemyView spawnedEnemyView = Instantiate(_spawnedEnemyView, _spawnedEnemyConteiner);
                spawnedEnemyView.Initialize(enemy.Name);
            }
        }
        else
        {
            foreach (var trap in roomData.Traps)
            {
                SpawnedEnemyView spawnedEnemyView = Instantiate(_spawnedEnemyView, _spawnedEnemyConteiner);
                spawnedEnemyView.Initialize(trap.gameObject.name);
            }
        }
    }
}