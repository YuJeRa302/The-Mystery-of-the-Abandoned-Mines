using System;

namespace Assets.Source.Game.Scripts
{
    public class KnowledgeBaseViewModel : IDisposable
    {
        private KnowledgeBaseModel _knowledgeBaseModel;
        private MenuModel _menuModel;

        public KnowledgeBaseViewModel(KnowledgeBaseModel knowledgeBaseModel, MenuModel menuModel)
        {
            _knowledgeBaseModel = knowledgeBaseModel;
            _menuModel = menuModel;
            _menuModel.InvokeKnowBaswShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
        }

        public event Action Showing;
        public event Action Hiding;

        public void Hide() => _menuModel.InvokeKnowledgeBaseHide();

        public void Dispose()
        {
            _knowledgeBaseModel.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}