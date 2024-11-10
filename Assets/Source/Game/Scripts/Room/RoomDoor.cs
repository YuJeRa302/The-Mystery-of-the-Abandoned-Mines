using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class RoomDoor : MonoBehaviour
    {
        [SerializeField] private Room _room;
        [SerializeField] private Animator _animator;
        [SerializeField] private Outline _outline;
        [SerializeField] private BoxCollider _colider;

        public event Action<RoomDoor> RoomDoorTaked;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Player player))
                SetDoorState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
                SetDoorState(false);
        }

        public void Lock() 
        {
            _colider.enabled = false;
            _outline.enabled = false;
            SetDoorState(false);
        }

        public void Unlock() 
        {
            _colider.enabled = true;
            _outline.enabled = true;
        }

        private void SetDoorState(bool state) 
        {
            if (gameObject.activeSelf == true)
                _animator.SetBool(DoorTransitionParameter.IsOpen.ToString(), state);
        }
    }
}