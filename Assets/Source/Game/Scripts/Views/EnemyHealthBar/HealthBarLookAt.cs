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


        // ��������� ����������� �� ������� � ������
        Vector3 direction = _camera.transform.position - transform.position;

        // ����������� ������ ���, ����� �� ������� � ������� ������
        transform.rotation = Quaternion.LookRotation(direction);

        // ���� �����, ����� ������ ��� ������������ ������ (�� ����������):
        transform.rotation = Quaternion.LookRotation(-transform.forward, _camera.transform.up);
    }
}