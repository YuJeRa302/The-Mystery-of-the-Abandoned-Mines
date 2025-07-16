using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    public abstract class KnowledgeData : ScriptableObject
    {
        [SerializeField] private KnowledgeView _knowledgeView;

        protected KnowledgeView KnowledgeView => _knowledgeView;

        public abstract void GetView(Transform conteiner, out List<KnowledgeView> knowledgeViews);
    }
}