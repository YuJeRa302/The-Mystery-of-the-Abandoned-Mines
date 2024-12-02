using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerAttacker _playerAttacker;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private CardDeck _cardDeck;
        [SerializeField] private Transform _weaponPoint;
        [SerializeField] private Transform _additionalWeaponPoint;
        [SerializeField] private WeaponView _weaponView;
        [SerializeField] private PlayerAnimation _playerAnimation;

        public CardDeck CardDeck => _cardDeck;
        public PlayerAttacker PlayerAttacker => _playerAttacker;
        public PlayerStats PlayerStats => _playerStats;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public WeaponView WeaponView => _weaponView;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
    }
}