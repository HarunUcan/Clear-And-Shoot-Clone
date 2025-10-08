using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
            gameObject.SetActive(false);
        }
    }

    public void IncreaseDamage(float amount)
    {
        _damage += amount;
        if (_damage < 0)
            _damage = 0;
    }
}
