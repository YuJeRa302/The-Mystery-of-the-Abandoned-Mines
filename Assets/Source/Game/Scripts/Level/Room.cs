using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Room : MonoBehaviour
    {
        private readonly int _rotateCount = 4;
        private readonly float _degreeRotation = 90.0f;

        [Header("Room Door Places")]
        [SerializeField] private GameObject _doorUpper;
        [SerializeField] private GameObject _doorRight;
        [SerializeField] private GameObject _doorDown;
        [SerializeField] private GameObject _doorLeft;

        public GameObject DoorUpper => _doorUpper;
        public GameObject DoorRight => _doorRight;
        public GameObject DoorDown => _doorDown;
        public GameObject DoorLeft => _doorLeft;

        public void RotateRandomly()
        {
            int count = Random.Range(0, _rotateCount);

            for (int index = 0; index < count; index++)
            {
                transform.Rotate(0, _degreeRotation, 0);

                GameObject temp = _doorLeft;
                _doorLeft = _doorDown;
                _doorDown = _doorRight;
                _doorRight = _doorUpper;
                _doorUpper = temp;
            }
        }
    }
}