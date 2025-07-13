using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Card
{
    public class CardSupportivParametrsView : MonoBehaviour
    {
        [SerializeField] private Image _iconParametr;

        public void Initialize(Sprite icon)
        {
            _iconParametr.sprite = icon;
        }
    }
}