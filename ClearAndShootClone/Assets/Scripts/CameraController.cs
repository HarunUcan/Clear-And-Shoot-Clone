using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _horizontalSpeed = 10f;
    [SerializeField] private Transform _leftBoundary;
    [SerializeField] private Transform _rightBoundary;

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, 0).normalized;
        transform.Translate(Vector3.forward * _speed * Time.fixedDeltaTime, Space.World);
        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = direction * _horizontalSpeed * Time.fixedDeltaTime;
            if (transform.position.x <= _leftBoundary.transform.position.x && move.x < 0) return;
            if (transform.position.x >= _rightBoundary.transform.position.x && move.x > 0) return;
            transform.Translate(move, Space.World);
        }
    }
}
