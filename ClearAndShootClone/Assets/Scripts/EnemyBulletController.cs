using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = -transform.forward * _speed;
    }
}
