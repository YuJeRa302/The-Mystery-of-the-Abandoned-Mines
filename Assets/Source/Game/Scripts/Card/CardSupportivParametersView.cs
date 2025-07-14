using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Card
{
    public class CardSupportivParametersView : MonoBehaviour
    {
        [SerializeField] private Image _iconParameter;

        public void Initialize(Sprite icon)
        {
            _iconParameter.sprite = icon;
        }
    }
}