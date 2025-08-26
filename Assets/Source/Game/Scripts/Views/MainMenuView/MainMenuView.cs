using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class MainMenuView : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();

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

        private CompositeDisposable _disposables = new ();
        private MenuModel _menuModel;
        private List<TipView> _tipViews = new ();
        private IEnumerator _getTips;
        private IEnumerator _animationTips;
        private IAudioPlayerService _audioPlayerService;
        private WaitForSeconds _delayNewTips;

        private void OnEnable()
        {
            CreateTip();
            GetRandomTip();
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(MenuModel menuModel, IAudioPlayerService audioPlayerService)
        {
            _menuModel = menuModel;
            _audioPlayerService = audioPlayerService;
            _delayNewTips = new WaitForSeconds(_timeNewTips);
            AddListener();
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
            _menuModel.InvokedMainMenuShowed += Show;

            MenuModel.Message
                .Receive<M_GamePaused>()
                .Subscribe(m => OnGamePaused(m.IsGamePaused))
                .AddTo(_disposables);

            MenuModel.Message
                .Receive<M_GameResumed>()
                .Subscribe(m => OnGameResumed(m.IsGameResumed))
                .AddTo(_disposables);
        }

        private void RemoveListener()
        {
            if (_disposables != null)
                _disposables.Dispose();

            _menuModel.InvokedMainMenuShowed -= Show;
            _menuModel.Dispose();
            _openUpgradesButton.onClick.RemoveListener(ShowUpgrades);
            _openSettingsButton.onClick.RemoveListener(ShowSettings);
            _openLevelsButton.onClick.RemoveListener(ShowLevels);
            _openWeaponsButton.onClick.RemoveListener(ShowWeapons);
            _openClassAbilityButton.onClick.RemoveListener(ShowClassAbility);
            _openKnowledgeBaseButton.onClick.RemoveListener(ShowKnowledgeBase);
            _openLeaderboardButton.onClick.RemoveListener(ShowLeaderboard);
        }

        private void ShowLeaderboard()
        {
            MessageBroker.Default.Publish(new M_LeaderboardShowed());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowKnowledgeBase()
        {
            MessageBroker.Default.Publish(new M_KnowledgeBaseShow());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowUpgrades()
        {
            MessageBroker.Default.Publish(new M_UpgradesShow());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowSettings()
        {
            MessageBroker.Default.Publish(new M_SettingsShow());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowLevels()
        {
            MessageBroker.Default.Publish(new M_LevelsShow());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowWeapons()
        {
            MessageBroker.Default.Publish(new M_WeaponsShow());
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
        }

        private void ShowClassAbility()
        {
            MessageBroker.Default.Publish(new M_ShowClassAbility());
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