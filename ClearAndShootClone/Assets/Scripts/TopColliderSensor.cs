using UnityEngine;

public class TopColliderSensor : MonoBehaviour
{
    public DestructablePlatformController ParentPlatform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            ParentPlatform.OnTopColliderHit(other);
        }
    }
}
