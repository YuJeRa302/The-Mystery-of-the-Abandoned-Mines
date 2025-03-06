using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public abstract class GamePanels : MonoBehaviour // Переделать по типу Model ViewModel View
    {
        protected Player Player;
        protected LevelObserver LevelObserver;

        public Action PanelOpened;
        public Action PanelClosed;
        public Action AdOpened;
        public Action AdClosed;

        public virtual void Initialize(Player player, LevelObserver levelObserver)
        {
            Player = player;
            LevelObserver = levelObserver;
        }

        protected virtual void Open()
        {
            gameObject.SetActive(true);
            //LevelObserver.PlayerView.gameObject.SetActive(false);
            PanelOpened?.Invoke();
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            //LevelObserver.PlayerView.gameObject.SetActive(true);
            PanelClosed?.Invoke();
        }
    }
}