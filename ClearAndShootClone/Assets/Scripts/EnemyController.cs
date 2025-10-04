using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    Queue<GameObject> _bulletPool = new Queue<GameObject>();

    void Start()
    {
        for(int i = 0; i < 5; i++)
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
        if(_bulletPool.Count > 0)
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
}
