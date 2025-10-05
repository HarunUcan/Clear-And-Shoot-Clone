using UnityEngine;

public class BulletRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // D�nd�rme h�z�
    public Vector3 RotationDirection = Vector3.forward; // D�nd�rme y�n�
    void Update()
    {
        transform.Rotate(RotationDirection * rotationSpeed * Time.deltaTime);
    }
}
