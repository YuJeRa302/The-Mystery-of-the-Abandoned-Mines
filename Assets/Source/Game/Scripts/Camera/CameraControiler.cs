using Assets.Source.Game.Scripts;
using Cinemachine;
using UnityEngine;

public class CameraControiler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner _cameraConfiner;
    [SerializeField] private Camera _camera;

    public void ChengeConfiner(Room room)
    {
        _cameraConfiner.m_BoundingVolume = room.Confiner;
    }

    public bool TrySeeDoor(GameObject leftWall)
    {
        Vector3 point = _camera.WorldToViewportPoint(leftWall.transform.position);

        if (point.x < 0.15f)
            return false;
        else
            return true;
    }
}