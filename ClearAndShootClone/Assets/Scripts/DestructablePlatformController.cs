using UnityEngine;

public class DestructablePlatformController : MonoBehaviour, IDamageable
{
    [SerializeField] private float _fallSpeed = 2f;   // Düşme hızı (Inspector'dan ayarlanabilir)
    private bool _isDamaged = false; // Hasar alıp almadığını kontrol eder
    [SerializeField] private float _fallDuration = 0.1f; // Hasar aldıktan sonra düşme süresi
    private float _timer;
    [SerializeField] private GameObject _topObject; // Düşman, Para vb...
    void Update()
    {
        if (_isDamaged)
        {
            _timer += Time.deltaTime;

            if (_timer <= _fallDuration)
                transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.World);
            else
            {
                _isDamaged = false;
                _timer = 0f;
            }

        }
    }

    public void OnTopColliderHit(Collider other)
    {
        _topObject.transform.SetParent(null);
        transform.GetComponent<Collider>().enabled = false;
        Debug.Log($"Üst collider zemine çarptı: {other.name}");
        Destroy(gameObject, .2f); // 2 saniye sonra platformu yok et
    }

    public void TakeDamage(float damage)
    {
        _isDamaged = true;
        _timer = 0f; // Zamanlayıcıyı sıfırla
    }
}
