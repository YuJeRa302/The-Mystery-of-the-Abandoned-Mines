using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerAttacker _playerAttacker;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private CardDeck _cardDeck;

        public CardDeck CardDeck => _cardDeck;
        public PlayerAttacker PlayerAttacker => _playerAttacker;
        public PlayerStats PlayerStats => _playerStats;
    }
}