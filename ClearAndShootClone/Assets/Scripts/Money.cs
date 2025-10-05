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



    public void Collect(Transform target)
    {

        FlyToTarget();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FlyToTarget()
    {
        float panelTransformCenterX = _panelTransform.rect.center.x;

        Vector3 panelTopPoint = _panelTransform.TransformPoint(new Vector2(panelTransformCenterX, _panelTransform.rect.yMax));

        _image.transform.position = panelTopPoint;

        panelTopPoint.z = 180f;
        Vector3 panelCenterPointWorld = Camera.main.ScreenToWorldPoint(panelTopPoint);
        transform.DOMove(panelCenterPointWorld, 1f).SetEase(Ease.InQuad);
    }

}
