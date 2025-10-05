using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    Queue<GameObject> _bulletPool = new Queue<GameObject>();
    public float InitHealth = 100f;
    private float _currentHealth;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private Material _deathMaterial;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = InitHealth;
        _healthText.text = _currentHealth.ToString("0");
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
        }

    }

    void Update()
    {
        
    }

    public void Fire()
    {
        if(_bulletPool.Count > 0 && _currentHealth > 0)
        {
            GameObject bullet = _bulletPool.Dequeue();
            bullet.transform.position = _firePoint.position;
            bullet.SetActive(true);
            StartCoroutine(ReturnBulletToPoolAfterTime(bullet, 2f));
        }
    }

    private IEnumerator ReturnBulletToPoolAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _healthText.text = _currentHealth.ToString("0");
        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _animator.SetTrigger("Death");
        GetComponentInChildren<Collider>().enabled = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            renderer.material = _deathMaterial;
        }
        StartCoroutine(DestroyAfterTime(3f));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
