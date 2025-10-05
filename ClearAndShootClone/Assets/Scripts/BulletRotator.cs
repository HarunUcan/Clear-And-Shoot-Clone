using UnityEngine;

public class BulletRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Döndürme hýzý
    public Vector3 RotationDirection = Vector3.forward; // Döndürme yönü
    void Update()
    {
        transform.Rotate(RotationDirection * rotationSpeed * Time.deltaTime);
    }
}
