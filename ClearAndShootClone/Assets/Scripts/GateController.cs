using TMPro;
using UnityEngine;

public class GateController : MonoBehaviour, IDamageable
{
    [SerializeField] private GradientImageGenerator _gradientImageGenerator;
    [SerializeField] private GateType _gateType;
    [SerializeField] private float _initValue;
    private float _currentValue;
    private float _oldValue;
    [SerializeField] private TMP_Text _valueText;

    public void TakeDamage(float damage)
    {
        _oldValue = _currentValue;
        _currentValue += damage / 10;
        if (_oldValue * _currentValue <= 0)
            _gradientImageGenerator.ApplyGradient(_gradientImageGenerator.GradientGreen);
        
        UpdateValue(_currentValue);
    }

    void Start()
    {
        Debug.Log("Selected GateType: " + _gateType);
        UpdateValue(_initValue);

        if (_initValue < 0)
            _gradientImageGenerator.ApplyGradient(_gradientImageGenerator.GradientRed);
        else
            _gradientImageGenerator.ApplyGradient(_gradientImageGenerator.GradientGreen);
    }

    void Update()
    {
        
    }

    private void UpdateValue(float newValue)
    {
        string gateType = string.Empty;
        switch (_gateType)
        {
            case GateType.Damage:
                gateType = "DAMAGE";
                break;
            case GateType.FireRate:
                gateType = "FIRERATE";
                break;
            case GateType.AddMoney:
                gateType = "MONEY";
                break;
            default:
                break;
        }
        _currentValue = newValue;
        string newValueStr = string.Empty;
        if (_currentValue > 0)
            newValueStr = "+" + _currentValue.ToString("0") + " " + gateType;
        else if (_currentValue < 0)
            newValueStr = _currentValue.ToString("0") + " " + gateType;
        else
            newValueStr = "+0 " + gateType;

        _valueText.text = newValueStr;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DangerZoneTrigger")
        {
            ApplyValue(other);
            transform.GetComponent<Collider>().enabled = false;
        }
    }

    private void ApplyValue(Collider other)
    {
        switch (_gateType)
        {
            case GateType.Damage:
                int weaponCount = GameManager.CollectedWeapons.Count;
                foreach (var w in GameManager.CollectedWeapons)
                {
                    w.GetComponent<Weapon>().IncreaseDamage(_currentValue / weaponCount);
                }
                break;
            case GateType.FireRate:
                // PlayerController.Instance.DecreaseFireRate(_currentValue);
                break;
            case GateType.AddMoney:
                // MoneyManager.Instance.AddMoney((int)_currentValue);
                break;
            default:
                break;
        }
    }
}
public enum GateType
{
    Damage,
    FireRate,
    AddMoney
}
