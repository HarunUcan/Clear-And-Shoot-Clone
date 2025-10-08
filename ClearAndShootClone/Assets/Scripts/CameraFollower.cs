using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    private Vector3 _offset;

    void Start()
    {
        _target = Camera.main.transform;
        _offset = transform.position - new Vector3(_target.position.x, transform.position.y, _target.position.z);
    }

    private void LateUpdate()
    {
        Vector3 targetPos = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        transform.position = targetPos + _offset;
    }

    void Update()
    {
        
    }
}
