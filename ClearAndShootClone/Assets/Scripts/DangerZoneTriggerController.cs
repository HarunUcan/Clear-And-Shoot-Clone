using UnityEngine;

public class DangerZoneTriggerController : MonoBehaviour
{
    private GameManager _gameManager;
    private Vector3 _offset;
    void Start()
    {
        _gameManager = GameManager.Instance;

        if (Camera.main != null)
        {
            _offset = transform.position - Camera.main.transform.position;
        }
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position + _offset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.tag == "DangerZone")
        {
            _gameManager.ChangeGameState();
        }

    }
}
