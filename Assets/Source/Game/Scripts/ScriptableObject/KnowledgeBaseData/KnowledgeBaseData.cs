using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBaseData", order = 51)]
    public class KnowledgeBaseData : ScriptableObject
    {
        [SerializeField] private List<SubcategoriesView> _playerSubcategoriesViews;
        [SerializeField] private List<SubcategoriesView> _gameSubcategoriesViews;
        [SerializeField] private List<SubcategoriesView> _enemySubcategoriesViews;

        public List<SubcategoriesView> PlayerSubcategoriesViews => _playerSubcategoriesViews;
        public List<SubcategoriesView> GameSubcategoriesViews => _gameSubcategoriesViews;
        public List<SubcategoriesView> EnemySubcategoriesViews => _enemySubcategoriesViews;
    }
}