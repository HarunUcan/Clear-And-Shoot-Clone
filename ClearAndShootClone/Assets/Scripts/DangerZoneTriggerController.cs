using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DangerZoneTriggerController : MonoBehaviour
{
    private GameManager _gameManager;
    private Vector3 _offset;
    [SerializeField] private Transform _target;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _offset = transform.position - new Vector3(_target.position.x, transform.position.y, _target.position.z);
    }

    private void LateUpdate()
    {
        Vector3 targetPos = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        transform.position = targetPos + _offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.tag == "DangerZone")
        {
            _gameManager.ChangeGameState();
        }

        else if (other != null && other.tag == "Money")
        {
            UIManager.Instance.FlyToUI(other.transform);
            GameManager.Instance.UpdateMoneyAmount(100);
        }

    }
}
