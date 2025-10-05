using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(_damage);
            gameObject.SetActive(false);
        }
    }
}
