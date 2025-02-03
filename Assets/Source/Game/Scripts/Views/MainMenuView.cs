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
        private IAudioPlayerService _audioPlayerService;
        private MainMenuViewModel _menuViewModel;

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(MainMenuViewModel menuViewModel, IAudioPlayerService audioPlayerService)
        {
            _menuViewModel = menuViewModel;
            _audioPlayerService = audioPlayerService;
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
            _menuViewModel.InvokedShow += Show;
        }

        private void RemoveListener() 
        {
            _openUpgradesButton.onClick.RemoveListener(ShowUpgrades);
            _openSettingsButton.onClick.RemoveListener(ShowSettings);
            _openLevelsButton.onClick.RemoveListener(ShowLevels);
            _openWeaponsButton.onClick.RemoveListener(ShowWeapons);
            _openClassAbilityButton.onClick.RemoveListener(ShowClassAbility);
            _menuViewModel.InvokedShow -= Show;
        }

        private void ShowUpgrades() 
        {
            _menuViewModel.InvokeUpgradesShow();
            gameObject.SetActive(false);
        }

        private void ShowSettings() 
        {
            _menuViewModel.InvokeSettingsShow();
            gameObject.SetActive(false);
        }

        private void ShowLevels() 
        {
            _menuViewModel.InvokeLevelsShow();
            gameObject.SetActive(false);
        }

        private void ShowWeapons() 
        {
            _menuViewModel.InvokeWeaponsShow();
            gameObject.SetActive(false);
        }

        private void ShowClassAbility()
        {
            _menuViewModel.InvokeClassAbilityShow();
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
            yield return new WaitForSeconds(_timeNewTips);
            ClearTip();
            CreateTip();
            GetRandomTip();
        }

        private void CreateTip()
        {
            TipView view = Instantiate(_tipView, _tipsContainer);
            view.Initialize(_tipsDatas[_rnd.Next(0, _tipsDatas.Count)]);
            _tipViews.Add(view);
            StartCoroutine(SetTipViewAnimation());
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
                _audioPlayerService.PlayOneShotPopupSound();

                view.transform.DOScale(_duration, _duration).
                    SetEase(Ease.OutBounce).
                    SetLink(view.gameObject);

                yield return new WaitForSeconds(_delay);
            }
        }

        private void Show() => gameObject.SetActive(true);
    }
}