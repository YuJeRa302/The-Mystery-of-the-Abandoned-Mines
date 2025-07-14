using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.ViewModels;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class MainMenuView : MonoBehaviour
    {
        private readonly System.Random _rnd = new();

        [SerializeField] private Button _openUpgradesButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _openLevelsButton;
        [SerializeField] private Button _openWeaponsButton;
        [SerializeField] private Button _openClassAbilityButton;
        [SerializeField] private Button _openLeaderboardButton;
        [SerializeField] private Button _openKnowledgeBaseButton;
        [Space(20)]
        [SerializeField] private List<TipData> _tipsDatas;
        [Space(20)]
        [SerializeField] private TipView _tipView;
        [SerializeField] private Transform _tipsContainer;
        [Header("[Tips Parameters]")]
        [SerializeField] private float _delay = 0.25f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _timeNewTips = 10;

        private List<TipView> _tipViews = new();
        private IEnumerator _getTips;
        private IEnumerator _animationTips;
        private IAudioPlayerService _audioPlayerService;
        private MainMenuViewModel _menuViewModel;
        private WaitForSeconds _delayNewTips;

        private void OnEnable()
        {
            if (_getTips != null)
                StopCoroutine(_getTips);

            if (_animationTips != null)
                StopCoroutine(_animationTips);

            CreateTip();
            GetRandomTip();
        }

        private void OnDestroy()
        {
            RemoveListener();
            _menuViewModel.Dispose();
        }

        public void Initialize(MainMenuViewModel menuViewModel, IAudioPlayerService audioPlayerService)
        {
            _menuViewModel = menuViewModel;
            _audioPlayerService = audioPlayerService;
            _delayNewTips = new WaitForSeconds(_timeNewTips);
            AddListener();

            if (_getTips != null)
                StopCoroutine(_getTips);

            if (_animationTips != null)
                StopCoroutine(_animationTips);

            CreateTip();
            GetRandomTip();
        }

        private void AddListener()
        {
            _openUpgradesButton.onClick.AddListener(ShowUpgrades);
            _openSettingsButton.onClick.AddListener(ShowSettings);
            _openLevelsButton.onClick.AddListener(ShowLevels);
            _openWeaponsButton.onClick.AddListener(ShowWeapons);
            _openClassAbilityButton.onClick.AddListener(ShowClassAbility);
            _openKnowledgeBaseButton.onClick.AddListener(ShowKnowledgeBase);
            _openLeaderboardButton.onClick.AddListener(ShowLeaderboard);
            _menuViewModel.Showing += Show;
            _menuViewModel.GamePaused += OnGamePaused;
            _menuViewModel.GameResumed += OnGameResumed;
        }

        private void RemoveListener()
        {
            _openUpgradesButton.onClick.RemoveListener(ShowUpgrades);
            _openSettingsButton.onClick.RemoveListener(ShowSettings);
            _openLevelsButton.onClick.RemoveListener(ShowLevels);
            _openWeaponsButton.onClick.RemoveListener(ShowWeapons);
            _openClassAbilityButton.onClick.RemoveListener(ShowClassAbility);
            _openKnowledgeBaseButton.onClick.RemoveListener(ShowKnowledgeBase);
            _openLeaderboardButton.onClick.RemoveListener(ShowLeaderboard);
            _menuViewModel.Showing -= Show;
            _menuViewModel.GamePaused -= OnGamePaused;
            _menuViewModel.GameResumed -= OnGameResumed;
        }

        private void ShowLeaderboard()
        {
            _menuViewModel.InvokeLeaderboardShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowKnowledgeBase()
        {
            _menuViewModel.InvokeKnowledgeBaseShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowUpgrades()
        {
            _menuViewModel.InvokeUpgradesShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowSettings()
        {
            _menuViewModel.InvokeSettingsShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowLevels()
        {
            _menuViewModel.InvokeLevelsShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowWeapons()
        {
            _menuViewModel.InvokeWeaponsShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowClassAbility()
        {
            _menuViewModel.InvokeClassAbilityShow();
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void GetRandomTip()
        {
            if (_getTips != null)
                StopCoroutine(_getTips);

            _getTips = LoadNewTip();
            StartCoroutine(_getTips);
        }

        private IEnumerator LoadNewTip()
        {
            yield return _delayNewTips;
            ClearTip();
            CreateTip();
            GetRandomTip();
        }

        private void CreateTip()
        {
            TipView view = Instantiate(_tipView, _tipsContainer);
            view.Initialize(_tipsDatas[_rnd.Next(0, _tipsDatas.Count)]);
            _tipViews.Add(view);

            if (_animationTips != null)
                StopCoroutine(_animationTips);

            _animationTips = SetTipViewAnimation();
            StartCoroutine(_animationTips);
        }

        private void ClearTip()
        {
            foreach (TipView view in _tipViews)
                Destroy(view.gameObject);

            _tipViews.Clear();
        }

        private IEnumerator SetTipViewAnimation()
        {
            foreach (TipView view in _tipViews)
            {
                view.transform.localScale = Vector3.zero;
            }

            foreach (TipView view in _tipViews)
            {
                view.transform.DOScale(_duration, _duration).
                    SetEase(Ease.OutBounce).
                    SetLink(view.gameObject);

                yield return new WaitForSeconds(_delay);
            }

            _audioPlayerService?.PlayOneShotPopupSound();
        }

        private void OnGamePaused(bool state) => Time.timeScale = Convert.ToInt32(state);

        private void OnGameResumed(bool state) => Time.timeScale = Convert.ToInt32(state);

        private void Show() => gameObject.SetActive(true);
    }
}