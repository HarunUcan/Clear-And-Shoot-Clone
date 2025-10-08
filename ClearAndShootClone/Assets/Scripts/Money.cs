using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Money : MonoBehaviour, ICollectable
{
    public float CleanPercentThreshold { get; set; } = 15f;

    [HideInInspector] public bool IsCleaned { get; set; } = false;

    // Başlangıç ve bitiş noktalarında bulunan kontrol noktaları temizlendi mi
    [HideInInspector] public bool IsStartPointPainted { get; set; } = false;
    [HideInInspector] public bool IsEndPointPainted { get; set; } = false;

    [SerializeField] private RectTransform _panelTransform;
    [SerializeField] private Image _image;

    [SerializeField] private Sprite _uiCoinSprite; // UI'da gösterilecek coin sprite'ı
    [SerializeField] private Transform _targetUI; // UI'daki para iconunun Transform'u



    public void Collect(Transform target)
    {
        GetComponentInChildren<Collider>().enabled = false;
        transform.parent = Camera.main.transform;

        transform.DOMoveY(10f, 1.2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            Vector3 targetScale = transform.localScale * .7f;
            transform.DOScale(targetScale, 1f).SetEase(Ease.InOutSine);
            transform.DORotate(new Vector3(0f, 0f, 90f), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
            transform.DOMove(target.position, .05f).SetEase(Ease.InOutSine);
        }).OnComplete(() =>
        {
            transform.DOScale(0f, .5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                UIManager.Instance.FlyToUI(transform);
                GameManager.Instance.UpdateMoneyAmount(100);
            });
        });

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


