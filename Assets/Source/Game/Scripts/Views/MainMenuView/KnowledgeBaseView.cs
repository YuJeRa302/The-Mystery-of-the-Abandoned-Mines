using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeBaseView : MonoBehaviour
{
    [SerializeField] private Transform _subcategoriesContainer;
    [SerializeField] private Transform _knowInfoConteiner;
    [Space(20)]
    [SerializeField] private Button _closeButton;
    [Space(20)]
    [SerializeField] private Button _playerInfo;
    [SerializeField] private Button _gameInfo;
    [SerializeField] private Button _enemyInfo;
    [Space(20)]
    [SerializeField] private SubcategoriesButtonView _subcategoriesButtonViewPrefab;
    [SerializeField] private KnowledgeBaseData _baseData;
    [Space(20)]
    [SerializeField] private GameObject _buttonSubCategories;
    [SerializeField] private GameObject _infoPanel;

    private List<SubcategoriesButtonView> _subcategoriesButtonViews = new List<SubcategoriesButtonView>();
    private List<KnowladgeView> _knowladgeViews = new List<KnowladgeView>();

    private KnowledgeBaseViewModel _knowledgeBaseViewModel;
    private IAudioPlayerService _audioPlayerService;

    private void OnDisable()
    {
        _buttonSubCategories.SetActive(false);
        _infoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        RemoveListener();
        _knowledgeBaseViewModel.Dispose();
    }

    public void Initialize(KnowledgeBaseViewModel knowledgeBaseViewModel, IAudioPlayerService audioPlayerService)
    {
        _knowledgeBaseViewModel = knowledgeBaseViewModel;
        _audioPlayerService = audioPlayerService;
        AddListener();
        _buttonSubCategories.SetActive(false);
        _infoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void AddListener()
    {
        _knowledgeBaseViewModel.InvokedShow += Show;
        _playerInfo.onClick.AddListener(() => ShowCatigories(_baseData.PlayerSubcategoriesViews));
        _gameInfo.onClick.AddListener(() => ShowCatigories(_baseData.GameSubcategoriesViews));
        _enemyInfo.onClick.AddListener(() => ShowCatigories(_baseData.EnemySubcategoriesViews));
        _closeButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void RemoveListener()
    {
        _knowledgeBaseViewModel.InvokedShow -= Show;
        _playerInfo.onClick.RemoveListener(() => ShowCatigories(_baseData.PlayerSubcategoriesViews));
        _gameInfo.onClick.RemoveListener(() => ShowCatigories(_baseData.GameSubcategoriesViews));
        _enemyInfo.onClick.RemoveListener(() => ShowCatigories(_baseData.EnemySubcategoriesViews));
        _closeButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    private void ShowCatigories(List<SubcategoriesView> subcategories)
    {
        Clear();
        _buttonSubCategories.SetActive(true);
        _infoPanel.SetActive(false);

        SubcategoriesButtonView subcategoriesButtonView;

        foreach (var subcategotiesView in subcategories)
        {
            subcategoriesButtonView = Instantiate(_subcategoriesButtonViewPrefab, _subcategoriesContainer);
            subcategoriesButtonView.Initialize(subcategotiesView.Name, subcategotiesView.Icon, subcategotiesView.KnowledgeData);
            subcategoriesButtonView.ChengetCategoru += ShowKnowledgeCategory;
            _subcategoriesButtonViews.Add(subcategoriesButtonView);
        }
    }

    private void ShowKnowledgeCategory(KnowledgeData knowledgeData)
    {
        ClearInfoConteiner();
        _infoPanel.SetActive(true);
        knowledgeData.GetView(_knowInfoConteiner, out List<KnowladgeView> knowView);
        _knowladgeViews = knowView;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnExitButtonClicked()
    {
        _audioPlayerService?.PlayOneShotButtonClickSound();
        _knowledgeBaseViewModel.Hide();
        gameObject.SetActive(false);
    }

    private void ClearInfoConteiner()
    {
        foreach (KnowladgeView view in _knowladgeViews)
        {
            Destroy(view.gameObject);
        }

        _knowladgeViews.Clear();
    }

    private void Clear()
    {
        foreach (SubcategoriesButtonView view in _subcategoriesButtonViews)
        {
            view.ChengetCategoru -= ShowKnowledgeCategory;
            Destroy(view.gameObject);
        }

        _subcategoriesButtonViews.Clear();
    }
}