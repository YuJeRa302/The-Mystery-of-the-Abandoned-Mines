using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    public abstract class KnowledgeData : ScriptableObject
    {
        [SerializeField] protected KnowledgeView _knowladgeView;

        public abstract void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews);
    }
}