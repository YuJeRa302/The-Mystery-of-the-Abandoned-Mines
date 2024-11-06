using Assets.Source.Game.Scripts;
using Cinemachine;
using UnityEngine;

public class CameraControiler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner _cameraConfiner;

    public void ChengeConfiner(Room room)
    {
        _cameraConfiner.m_BoundingVolume = room.Confiner;
    }
}