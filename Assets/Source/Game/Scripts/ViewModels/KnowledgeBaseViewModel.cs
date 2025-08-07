using Assets.Source.Game.Scripts.Models;
using System;

namespace Assets.Source.Game.Scripts.ViewModels
{
    public class KnowledgeBaseViewModel
    {
        private KnowledgeBaseModel _knowledgeBaseModel;
        private MenuModel _menuModel;

        public KnowledgeBaseViewModel(KnowledgeBaseModel knowledgeBaseModel, MenuModel menuModel)
        {
            _knowledgeBaseModel = knowledgeBaseModel;
            _menuModel = menuModel;
            _menuModel.InvokeKnowBaswShowed += () => Showing?.Invoke();
        }

        public event Action Showing;

        public void Hide() => _menuModel.InvokeKnowledgeBaseHide();
    }
}