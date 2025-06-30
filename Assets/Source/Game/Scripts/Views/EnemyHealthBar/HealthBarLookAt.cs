using UnityEngine;

public class HealthBarLookAt : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        Vector3 direction = _camera.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.LookRotation(-transform.forward, _camera.transform.up);
    }
}