using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class MainMenuView : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();

        [SerializeField] private Button _openUpgradesButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _openLevelsButton;
        [SerializeField] private Button _openWeaponsButton;
        [SerializeField] private Button _openClassAbilityButton;
        [SerializeField] private Button _opneKnowledgeBaseButton;
        [Space(20)]
        [SerializeField] private List<TipData> _tipsDatas;
        [Space(20)]
        [SerializeField] private TipView _tipView;
        [SerializeField] private Transform _tipsContainer;
        [Header("[Tips Parameters]")]
        [SerializeField] private float _delay = 0.25f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _timeNewTips = 10;

        private List<TipView> _tipViews = new ();
        private IEnumerator _getTips;
        private IEnumerator _animationTips;
        private IAudioPlayerService _audioPlayerService;
        private MainMenuViewModel _menuViewModel;
        private DG.Tweening.Sequence _sequence;
        private WaitForSeconds _delayNewTrips;

        private void OnEnable()
        {
            _sequence.Kill();

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
            _delayNewTrips = new WaitForSeconds(_timeNewTips);
            AddListener();
            _sequence.Kill();

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
            _opneKnowledgeBaseButton.onClick.AddListener(ShowKnowledgeBase);
            _menuViewModel.InvokedShow += Show;
        }

        private void RemoveListener() 
        {
            _openUpgradesButton.onClick.RemoveListener(ShowUpgrades);
            _openSettingsButton.onClick.RemoveListener(ShowSettings);
            _openLevelsButton.onClick.RemoveListener(ShowLevels);
            _openWeaponsButton.onClick.RemoveListener(ShowWeapons);
            _openClassAbilityButton.onClick.RemoveListener(ShowClassAbility);
            _opneKnowledgeBaseButton.onClick.RemoveListener(ShowKnowledgeBase);
            _menuViewModel.InvokedShow -= Show;
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
            yield return _delayNewTrips;
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
            WaitForSeconds delay = new WaitForSeconds(_delay);

            foreach (TipView view in _tipViews)
            {
                view.transform.localScale = Vector3.zero;
            }

            _sequence = DOTween.Sequence();

            foreach (TipView view in _tipViews)
            {
                _sequence.Append(view.transform.DOScale(_duration, _duration).
                    SetEase(Ease.OutBounce).
                    SetLink(view.gameObject));

                yield return delay;
            }

                _audioPlayerService?.PlayOneShotPopupSound();
        }

        private void Show() => gameObject.SetActive(true);
    }
}