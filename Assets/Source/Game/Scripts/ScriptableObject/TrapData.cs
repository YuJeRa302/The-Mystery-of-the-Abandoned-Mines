using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Trap", menuName = "Create TrapData", order = 51)]
    public class TrapData : ScriptableObject
    {
        [SerializeField] private GameObject _trapPrefab;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        public GameObject TrapPrafab => _trapPrefab;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
    }
}