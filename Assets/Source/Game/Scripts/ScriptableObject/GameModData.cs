using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Game Mode Data", menuName = "Create Game Mode Data", order = 51)]
    public class GameModData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private string _access;

        public Sprite Icon => _icon;
        public string Name => _name;
        public string Description => _description;
        public string Access => _access;
    }
}