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
        //transform.LookAt(_camera.transform);
        //transform.LookAt(new Vector3(0, _camera.transform.position.y + 270f, 0));
        //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);

        //transform.LookAt(_camera.transform);
        //Vector3 direction = _camera.transform.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(direction);


        // Вычисляем направление от канваса к камере
        Vector3 direction = _camera.transform.position - transform.position;

        // Ориентируем канвас так, чтобы он смотрел в сторону камеры
        transform.rotation = Quaternion.LookRotation(direction);

        // Если нужно, чтобы канвас был параллельным камере (не наклонялся):
        transform.rotation = Quaternion.LookRotation(-transform.forward, _camera.transform.up);
    }
}