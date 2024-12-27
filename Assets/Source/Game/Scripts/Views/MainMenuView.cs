using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _openUpgradesButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _openLevelsButton;
        [SerializeField] private Button _openWeaponsButton;

        private MainMenuViewModel _menuViewModel;

        private void OnDestroy()
        {
            _openUpgradesButton.onClick.RemoveListener(ShowUpgrades);
            _openSettingsButton.onClick.RemoveListener(ShowSettings);
            _openLevelsButton.onClick.RemoveListener(ShowLevels);
            _openWeaponsButton.onClick.RemoveListener(ShowWeapons);
            _menuViewModel.InvokedShow -= Show;
        }

        public void Initialize(MainMenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
            _openUpgradesButton.onClick.AddListener(ShowUpgrades);
            _openSettingsButton.onClick.AddListener(ShowSettings);
            _openLevelsButton.onClick.AddListener(ShowLevels);
            _openWeaponsButton.onClick.AddListener(ShowWeapons);
            _menuViewModel.InvokedShow += Show;
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

        private void Show() => gameObject.SetActive(true);
    }
}