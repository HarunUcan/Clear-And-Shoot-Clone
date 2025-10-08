using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private Sprite _uiCoinSprite; // UI'da gösterilecek coin sprite'ı
    [SerializeField] private Transform _targetUI; // UI'daki para iconunun Transform'u
    [SerializeField] private float _flyDuration = 0.5f; // Uçuş
    [SerializeField] private TMP_Text _moneyText; // Para miktarını gösteren Text

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void FlyToUI(Transform collectedObject)
    {
        Debug.Log("FlyToUI called");
        // Coin sprite'ının UI’a doğru uçması için world-to-screen konum dönüşümü
        Vector3 screenPos = Camera.main.WorldToScreenPoint(collectedObject.position);

        // Canvas üstüne geçici bir UI coin objesi oluştur
        GameObject uiCoin = new GameObject("UICoin");
        uiCoin.transform.SetParent(GameObject.Find("UICanvas").transform, false);

        RectTransform rect = uiCoin.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80, 80);
        uiCoin.transform.position = screenPos;

        Image img = uiCoin.AddComponent<Image>();
        img.sprite = _uiCoinSprite;

        // Hedef pozisyon (UI'daki para iconunun pozisyonu)
        Vector3 targetPos = _targetUI.position;

        // DOTween animasyonu
        rect.DOMove(targetPos, _flyDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Parayı artır
                //MoneyManager.Instance.AddMoney(1);

                // Coin UI'ını sil
                GameObject.Destroy(uiCoin);

                // Sahnedeki coin objesini de yok et
                GameObject.Destroy(collectedObject.gameObject);
            });
    }

    public void UpdateMoneyText(int newAmount)
    {
        _moneyText.text = newAmount.ToString();
    }
}
