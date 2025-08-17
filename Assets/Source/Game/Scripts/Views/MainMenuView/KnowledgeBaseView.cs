using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
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

        private List<SubcategoriesButtonView> _subcategoriesButtonViews = new ();
        private List<KnowledgeView> _knowladgeViews = new ();
        private IAudioPlayerService _audioPlayerService;
        private CompositeDisposable _disposables = new ();

        private void OnDisable()
        {
            _buttonSubCategories.SetActive(false);
            _infoPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            AddListener();
            _buttonSubCategories.SetActive(false);
            _infoPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        private void AddListener()
        {
            MessageBroker.Default
                .Receive<M_KnowledgeBaseShow>()
                .Subscribe(m => Show())
                .AddTo(_disposables);

            _playerInfo.onClick.AddListener(() => ShowCatigories(_baseData.PlayerSubcategoriesViews));
            _gameInfo.onClick.AddListener(() => ShowCatigories(_baseData.GameSubcategoriesViews));
            _enemyInfo.onClick.AddListener(() => ShowCatigories(_baseData.EnemySubcategoriesViews));
            _closeButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void RemoveListener()
        {
            if (_disposables != null)
                _disposables.Dispose();

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
                subcategoriesButtonView.Initialize(subcategotiesView.Name,
                    subcategotiesView.Icon, subcategotiesView.KnowledgeData);
                subcategoriesButtonView.CategoryChanged += ShowKnowledgeCategory;
                _subcategoriesButtonViews.Add(subcategoriesButtonView);
            }
        }

        private void ShowKnowledgeCategory(KnowledgeData knowledgeData)
        {
            ClearInfoConteiner();
            _infoPanel.SetActive(true);
            knowledgeData.GetView(_knowInfoConteiner, out List<KnowledgeView> knowView);
            _knowladgeViews = knowView;
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void OnExitButtonClicked()
        {
            _audioPlayerService?.PlayOneShotButtonClickSound();
            MessageBroker.Default.Publish(new M_Hide());
            gameObject.SetActive(false);
        }

        private void ClearInfoConteiner()
        {
            foreach (KnowledgeView view in _knowladgeViews)
            {
                Destroy(view.gameObject);
            }

            _knowladgeViews.Clear();
        }

        private void Clear()
        {
            foreach (SubcategoriesButtonView view in _subcategoriesButtonViews)
            {
                view.CategoryChanged -= ShowKnowledgeCategory;
                Destroy(view.gameObject);
            }

            _subcategoriesButtonViews.Clear();
        }
    }
}