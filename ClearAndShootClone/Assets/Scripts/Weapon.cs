using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Temizlenmi� say�laca�� y�zde
    public float CleanPercentThreshold = 15f;
    //A��ld��� seviye
    public int UnlockedLevel = 0;
    
    public GameObject Ammo;

    [HideInInspector] public bool IsCollected { get; set; } = false;

    // Ba�lang��, orta ve biti� noktalar�nda bulunan kontrol noktalar� temizlendi mi
    [HideInInspector] public bool IsStartPointPainted { get; set; } = false;
    [HideInInspector] public bool IsEndPointPainted { get; set; } = false;

    void LateUpdate()
    {
        if (transform.parent != null && Camera.main != null)
        {
            // D�nya y�n�nde sabit forward:
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    }

    public void WeaponCheckPointSetStatus(WeaponCheckPoint point, bool status)
    {
        switch (point)
        {
            case WeaponCheckPoint.StartPoint:
                IsStartPointPainted = status;
                break;
            case WeaponCheckPoint.EndPoint:
                IsEndPointPainted = status;
                break;
            default:
                break;
        }
    }    

}

public enum WeaponCheckPoint
{
    StartPoint,
    EndPoint
}