using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, ICollectable
{
    // Temizlenmiþ sayýlacaðý yüzde
    public float CleanPercentThreshold { get; set; } = 15f;
    //Açýldýðý seviye
    public int UnlockedLevel = 0;
    
    public GameObject BulletPrefab;
    private Queue<GameObject> _bulletPool = new Queue<GameObject>();
    private int _poolSize = 10;

    [HideInInspector] public bool IsCleaned { get; set; } = false;

    // Baþlangýç ve bitiþ noktalarýnda bulunan kontrol noktalarý temizlendi mi
    [HideInInspector] public bool IsStartPointPainted { get; set; } = false;
    [HideInInspector] public bool IsEndPointPainted { get; set; } = false;

    public bool IsFireable { get; set; } = false;
    private bool _animStarted = false;
    private float _fireCooldown = 0.5f;
    private float _timer = 0f;

    [SerializeField] private GameObject _meshTop;
    [SerializeField] private GameObject _meshBottom;

    [Header("Animation Settings")]
    [SerializeField] private bool _isRotationAnimated = false;
    [SerializeField] private bool _isMovingAnimated = false;
    [SerializeField] private Vector3 _rotationAnimDirection = Vector3.forward;
    [SerializeField] private Vector3 _movingAnimDirection = Vector3.up;

    private void Update()
    {
        if (IsFireable)
        {
            if (!_animStarted)
            {
                _animStarted = true;
                if (_isRotationAnimated)
                {
                    // Döndürme Animasyonu

                    _meshTop.transform.DOLocalRotate(_rotationAnimDirection * 45f, 0.2f, RotateMode.LocalAxisAdd)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);
                    _meshBottom.transform.DOLocalRotate(_rotationAnimDirection * 45f, 0.2f, RotateMode.LocalAxisAdd)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);
                }
                if (_isMovingAnimated)
                {
                    // Geri Tepme Hareket Animasyonu
                    transform.DOMove(transform.position + _movingAnimDirection, 1f)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetEase(Ease.InOutSine);
                }
            }
            _timer += Time.deltaTime;
            if (_timer >= _fireCooldown)
            {
                Fire();
                _timer = 0f;
            }
        }
    }

    void LateUpdate()
    {
        if (transform.parent != null && Camera.main != null)
        {
            // Dünya yönünde sabit forward:
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    }

    public void IncreaseDamage(float amount)
    {
        foreach (var bullet in _bulletPool)
        {
            if (bullet.TryGetComponent<BulletController>(out BulletController bulletController))
            {
                bulletController.IncreaseDamage(amount);
            }
        }
        Debug.Log("Increased bullet damage by: " + amount);
    }

    public void Collect(Transform target)
    {
        GetComponentInChildren<Collider>().enabled = false;
        transform.parent = Camera.main.transform;
        WaterGunController.CurrentCollectedWeaponIndex++;

        GameManager.CollectedWeapons.Add(gameObject);

        transform.DOMoveY(10f, 1.2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            Vector3 targetScale = transform.localScale * .7f;
            transform.DOScale(targetScale, 1f).SetEase(Ease.InOutSine);
            transform.DORotate(new Vector3(0f, 0f, 90f), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
            transform.DOMove(target.position, .05f).SetEase(Ease.InOutSine);
        });
    }

    public void Fire()
    {
        if(BulletPrefab != null)
        {
            if (_bulletPool.Count == 0)
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    GameObject bullet = Instantiate(BulletPrefab);
                    bullet.SetActive(false);
                    _bulletPool.Enqueue(bullet);
                }
            }
            GameObject pooledBullet = _bulletPool.Dequeue();
            pooledBullet.transform.position = transform.position + transform.up * 0.5f; // Silahýn ucundan biraz ileri
            pooledBullet.transform.rotation = transform.rotation;
            pooledBullet.SetActive(true);
            // Mermiye ileri doðru hareket ekle
            Rigidbody rb = pooledBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * 20f; // Ýleri doðru hýz
            }
            // Mermiyi belirli bir süre sonra havuza geri koy
            StartCoroutine(ReturnBulletToPool(pooledBullet, 2f));
        }
        else
        {
            // TODO: Mermisi olmayan yakýn hasar veren silahlar için ateþleme bölümü
        }
    }
    private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }

    public void ChangeState()
    {
        if (IsFireable)
        {
            IsFireable = false;
            _animStarted = false;
            transform.DOKill();
            if(_meshTop != null && _meshBottom != null)
            {
                _meshTop.transform.DOKill();
                _meshBottom.transform.DOKill();
            }
            //transform.localRotation = Quaternion.identity;
            //transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
        }
        else
        {
            IsFireable = true;
        }
    }
}