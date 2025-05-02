using Assets.Source.Game.Scripts;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class EnemyKnowladgeView : KnowladgeView
{
    [SerializeField] private Image _icon;
    [SerializeField] private LeanLocalizedText _name;
    [SerializeField] private LeanLocalizedText _description;
    [SerializeField] private EnemyStatsView _statsViewPrefab;
    [SerializeField] private Transform _statsConteiner;

    public void Initialize(EnemyData enemyData)
    {
        _icon.sprite = enemyData.Icon;
        _name.TranslationName = enemyData.Name;
        _description.TranslationName = enemyData.Descroption;

        InstantieteEnemyStats(nameof(enemyData.Damage), enemyData.Damage.ToString());
        InstantieteEnemyStats(nameof(enemyData.Health), enemyData.Health.ToString());
        InstantieteEnemyStats(nameof(enemyData.GoldReward), enemyData.GoldReward.ToString());
        InstantieteEnemyStats(nameof(enemyData.ExperienceReward), enemyData.ExperienceReward.ToString());
        InstantieteEnemyStats(nameof(enemyData.ChanceSpawn), enemyData.ChanceSpawn.ToString());
    }

    private void InstantieteEnemyStats(string nameStats, string value)
    {
        EnemyStatsView enemyStatsView = Instantiate(_statsViewPrefab, _statsConteiner);
        enemyStatsView.Initialize(nameStats, value);
    }
}