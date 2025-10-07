using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

public class FinalMoneyPlatformController : MonoBehaviour, IDamageable
{
    private bool _isDoTweenAnimPlaying = false;
    private float _currentScaleZ;
    private float _currentScaleX;

    [SerializeField] private GameObject _topObject; // Düþman, Para vb...

    [SerializeField] private float _platformInitHealth = 100f;
    private float _currentPlatformHealth;
    [SerializeField] private TMP_Text _platformHealthText;
    public void TakeDamage(float damage)
    {
        
        _currentPlatformHealth -= damage;
        if (_currentPlatformHealth <= 0)
        {
            // tüm tweens'leri öldür
            DOTween.Kill(transform);
            DOTween.Kill(_platformHealthText);
            _topObject.GetComponent<Rigidbody>().useGravity = true;
            _topObject.transform.SetParent(null);
            Destroy(gameObject);
        }
        
        if (!_isDoTweenAnimPlaying)
        {
            _isDoTweenAnimPlaying = true;

            transform.DOScaleZ(_currentScaleZ * 1.2f, 0.2f).SetEase(Ease.InBack);
            transform.DOScaleX(_currentScaleX * 1.2f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ResetScale(.2f);
            });
        }

        HealthTxtUpdateWithDoTween(_currentPlatformHealth);

    }

    void Start()
    {
        _currentPlatformHealth = _platformInitHealth;
        _currentScaleZ = transform.localScale.z;
        _currentScaleX = transform.localScale.x;
    }

    void Update()
    {      

    }

    private void ResetScale(float duration)
    {
        transform.DOScaleZ(_currentScaleZ, duration).SetEase(Ease.InBack);
        transform.DOScaleX(_currentScaleX, duration).SetEase(Ease.InBack).OnComplete(() =>
        {
            _isDoTweenAnimPlaying = false;
        });
    }

    private void HealthTxtUpdateWithDoTween(float health)
    {
            // kill existing tweens on the text to avoid overlapping animations
            DOTween.Kill(_platformHealthText);
            float currentHealth = float.Parse(_platformHealthText.text);
            DOTween.To(() => currentHealth, x => currentHealth = x, health, 0.2f).OnUpdate(() =>
            {
                _platformHealthText.text = ((int)currentHealth).ToString();
            }).OnComplete(() =>
            {
            });
        
    }
}
