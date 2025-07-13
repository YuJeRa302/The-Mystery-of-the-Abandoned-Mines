using Assets.Source.Game.Scripts.Characters;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class EnemyKnowladgeView : KnowladgeView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private EnemyStatsView _statsViewPrefab;
        [SerializeField] private Transform _statsContainer;

        private int _maxTireIndex = 4;

        public void Initialize(EnemyData enemyData)
        {
            _icon.sprite = enemyData.Icon;
            _name.TranslationName = enemyData.Name;
            _description.TranslationName = enemyData.Descroption;

            InstantieteEnemyStats(nameof(EnemyStats.Damage), enemyData.EnemyStats[_maxTireIndex].Damage.ToString());
            InstantieteEnemyStats(nameof(EnemyStats.Health), enemyData.EnemyStats[_maxTireIndex].Health.ToString());
            InstantieteEnemyStats(nameof(EnemyStats.GoldReward),
                enemyData.EnemyStats[_maxTireIndex].GoldReward.ToString());
            InstantieteEnemyStats(nameof(EnemyStats.ExperienceReward),
                enemyData.EnemyStats[_maxTireIndex].ExperienceReward.ToString());
            InstantieteEnemyStats(nameof(EnemyStats.ChanceSpawn),
                enemyData.EnemyStats[_maxTireIndex].ChanceSpawn.ToString());
        }

        private void InstantieteEnemyStats(string nameStats, string value)
        {
            EnemyStatsView enemyStatsView = Instantiate(_statsViewPrefab, _statsContainer);
            enemyStatsView.Initialize(nameStats, value);
        }
    }
}