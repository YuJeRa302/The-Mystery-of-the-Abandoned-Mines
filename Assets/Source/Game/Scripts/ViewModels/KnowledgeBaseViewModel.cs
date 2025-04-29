using System;

public class KnowledgeBaseViewModel : IDisposable
{
    private KnowledgeBaseModel _knowledgeBaseModel;
    private MenuModel _menuModel;

    public KnowledgeBaseViewModel(KnowledgeBaseModel knowledgeBaseModel, MenuModel menuModel)
    {
        _knowledgeBaseModel = knowledgeBaseModel;
        _menuModel = menuModel;
        _menuModel.InvokeKnowBaswShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
    }

    public event Action InvokedShow;
    public event Action InvokedHide;

    public void Hide() => _menuModel.InvokeUpgradesHide();

    public void Dispose()
    {
        _knowledgeBaseModel.Dispose();
        GC.SuppressFinalize(this);
    }
}