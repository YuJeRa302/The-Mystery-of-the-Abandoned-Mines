using Assets.Source.Game.Scripts.Views;
using Cinemachine;
using UnityEngine;

namespace Assets.Source.Game.Scripts.CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineConfiner _cameraConfiner;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Camera _camera;
        [SerializeField] private VariableJoystick _variableJoystick;

        private Transform _target;
        private float _viewportLeftThreshold = 0.15f;

        public Camera Camera => _camera;
        public VariableJoystick VariableJoystick => _variableJoystick;

        public void SetLookTarget(Transform target)
        {
            _target = target;
            _virtualCamera.Follow = _target;
        }

        public void ChangeConfiner(RoomView room)
        {
            _cameraConfiner.m_BoundingVolume = room.Confiner;
        }

        public bool TrySeeDoor(GameObject leftWall)
        {
            Vector3 point = _camera.WorldToViewportPoint(leftWall.transform.position);

            if (point.x < _viewportLeftThreshold)
                return false;
            else
                return true;
        }
    }
}