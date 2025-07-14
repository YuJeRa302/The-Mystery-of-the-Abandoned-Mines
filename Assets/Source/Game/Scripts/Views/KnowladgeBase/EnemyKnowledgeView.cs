using Assets.Source.Game.Scripts.Characters;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class EnemyKnowledgeView : KnowledgeView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private EnemyStatsView _statsViewPrefab;
        [SerializeField] private Transform _statsContainer;

        private int _maxTierIndex = 4;

        public void Initialize(EnemyData enemyData)
        {
            _icon.sprite = enemyData.Icon;
            _name.TranslationName = enemyData.Name;
            _description.TranslationName = enemyData.Description;

            InstantiateEnemyStats(nameof(EnemyStats.Damage),
                enemyData.EnemyStats[_maxTierIndex].Damage.ToString());
            InstantiateEnemyStats(nameof(EnemyStats.Health),
                enemyData.EnemyStats[_maxTierIndex].Health.ToString());
            InstantiateEnemyStats(nameof(EnemyStats.GoldReward),
                enemyData.EnemyStats[_maxTierIndex].GoldReward.ToString());
            InstantiateEnemyStats(nameof(EnemyStats.ExperienceReward),
                enemyData.EnemyStats[_maxTierIndex].ExperienceReward.ToString());
            InstantiateEnemyStats(nameof(EnemyStats.ChanceSpawn),
                enemyData.EnemyStats[_maxTierIndex].ChanceSpawn.ToString());
        }

        private void InstantiateEnemyStats(string nameStats, string value)
        {
            EnemyStatsView enemyStatsView = Instantiate(_statsViewPrefab, _statsContainer);
            enemyStatsView.Initialize(nameStats, value);
        }
    }
}