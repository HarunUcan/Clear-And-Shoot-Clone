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
}
